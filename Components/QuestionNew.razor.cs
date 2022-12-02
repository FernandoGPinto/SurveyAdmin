using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SurveyAdmin.Data;
using SurveyAdmin.Interfaces;
using SurveyAdmin.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Components
{
    public partial class QuestionNew
    {
        [CascadingParameter] public BlazoredModalInstance ModalInstance { get; set; }
        [Inject] public IPCDQuestionRepository PCDQuestionRepository { get; set; }
        [Inject] public AuthenticationStateProvider Provider { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }

        private PCDQuestionDto Question;

        private bool Sql { get; set; } = true;

        private bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            // Instantiate new PCDQuestionDto and populate RecordUpdatedBy with name of current user.
            var authenticationState = await Provider.GetAuthenticationStateAsync();

            string userName = authenticationState.User.Claims.Where(x => x.Type == "name").FirstOrDefault().Value ?? "";

            Question = new() { RecordUpdatedBy = userName };
        }

        /// <summary>
        /// Show or hide SQL Query comment box.
        /// </summary>
        /// <param name="e"></param>
        private void ShowSql(ChangeEventArgs e)
        {
            Sql = e.Value.ToString() != "True";
        }

        /// <summary>
        /// Submits the new question.
        /// </summary>
        /// <returns></returns>
        private async Task SubmitQuestion()
        {
            IsLoading = true;

            try
            {
                // Get format id.
                var formatId = await PCDQuestionRepository.GetFormatIdAsync(Question.QuestionFormat);

                // Generate new PCDQuestion with format id.
                PCDQuestion pcdQuestion = new() { Question = Question.Question, QuestionFormatId = formatId, IsClientQuestion = Convert.ToBoolean(Question.IsClientQuestion), IsGenericQuestion = Convert.ToBoolean(Question.IsGenericQuestion), IsSQLQuery = Convert.ToBoolean(Question.IsSQLQuery), SQLQuery = Question.SQLQuery, HasFurtherComment = Convert.ToBoolean(Question.HasFurtherComment), RecordUpdatedBy = Question.RecordUpdatedBy };

                // Insert question.
                await PCDQuestionRepository.InsertQuestionAsync(pcdQuestion);

                // Once inserted, close modal.
                await ModalInstance.CloseAsync(ModalResult.Ok(Question.Question));
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Add Question", "Please try again or contact I.T. support.", "error");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
    }
}
