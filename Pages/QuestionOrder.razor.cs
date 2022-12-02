using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SurveyAdmin.Data;
using SurveyAdmin.Interfaces;
using SurveyAdmin.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Pages
{
    public partial class QuestionOrder
    {
        [Inject] public IPCDQuestionRepository PCDQuestionRepository { get; set; }
        [Inject] public UnitOfWork UnitOfWork { get; set; }
        [Inject] public AuthenticationStateProvider Provider { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }

        private int currentIndex;

        private List<PCDQuestionDto> Questions;

        private bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Get list of all questions.
                Questions = (await PCDQuestionRepository.GetPCDQuestionsAsync()).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Load Questions", "Please try again or contact I.T. support.", "error");
            }
        }

        private void StartDrag(PCDQuestionDto question)
        {
            currentIndex = GetIndex(question);
        }

        private int GetIndex(PCDQuestionDto question)
        {
            return Questions.FindIndex(a => a.Id == question.Id);
        }

        private void Drop(PCDQuestionDto question)
        {
            var index = GetIndex(question);

            // Get current question.
            var current = Questions[currentIndex];

            // Remove question from current index.
            Questions.RemoveAt(currentIndex);
            Questions.Insert(index, current);

            // Update current selection.
            currentIndex = index;

            StateHasChanged();
        }

        /// <summary>
        /// Saves changes to question order.
        /// </summary>
        /// <returns></returns>
        private async Task SaveChanges()
        {
            // Confirm whether user wants to edit question.
            var confirmation = await Swal.FireAsync(new SweetAlertOptions { Title = "Are you sure you want to save changes?", Icon = SweetAlertIcon.Info, ShowCancelButton = true });

            if (confirmation.IsConfirmed)
            {
                IsLoading = true;

                // Populate RecordUpdatedBy with name of current user.
                var authenticationState = await Provider.GetAuthenticationStateAsync();

                var recordUpdatedBy = authenticationState.User.Claims.Where(x => x.Type == "name").FirstOrDefault().Value ?? "";

                int i = 1;
                foreach (var question in Questions)
                {
                    question.Index = i;
                    question.RecordUpdatedBy = recordUpdatedBy;

                    i++;
                }

                // Send request for updating questions in the database.
                var response = await UpdateQuestionsAsync(Questions);

                IsLoading = false;

                // Trigger component re-rendering at this stage to ensure that the loading spinner is removed before the swal modal pop up. ComponentBase cannot re-render until the modal is closed and the modal cannot be closed while the loading spinner is on.
                StateHasChanged();

                if (response)
                {
                    await Swal.FireAsync("Your changes have been saved!", null, "success");
                }
                else
                {
                    await Swal.FireAsync("Unable to Save Changes", "Please try again or contact I.T. support.", "error");
                }
            }
        }

        /// <summary>
        /// Generates a new PCDQuestion object and calls the PCDQuestionRepository to update the relevant questions with the new "Index" order.
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private async Task<bool> UpdateQuestionsAsync(List<PCDQuestionDto> questions)
        {
            List<PCDQuestion> pcdQuestions = new();

            foreach (var question in questions)
            {
                // Get format id.
                var formatId = await PCDQuestionRepository.GetFormatIdAsync(question.QuestionFormat);

                // Generate new PCDQuestion with format id.
                PCDQuestion pcdQuestion = new() { Id = question.Id, Index = question.Index, Question = question.Question, QuestionFormatId = formatId, IsClientQuestion = Convert.ToBoolean(question.IsClientQuestion), IsGenericQuestion = Convert.ToBoolean(question.IsGenericQuestion), IsSQLQuery = Convert.ToBoolean(question.IsSQLQuery), SQLQuery = question.SQLQuery, HasFurtherComment = Convert.ToBoolean(question.HasFurtherComment), RecordUpdatedBy = question.RecordUpdatedBy };

                pcdQuestions.Add(pcdQuestion);
            }

            try
            {
                UnitOfWork.BeginTransaction();
                await PCDQuestionRepository.UpdateQuestionsAsync(pcdQuestions);
                UnitOfWork.Commit();

                return true;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                Log.Error(e, e.Message);

                return false;
            }
        }
    }
}
