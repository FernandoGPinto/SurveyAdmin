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
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Components
{
    public partial class QuestionEdit
    {
        [CascadingParameter] public BlazoredModalInstance ModalInstance { get; set; }
        [Parameter] public PCDQuestionDto Question { get; set; }
        [Inject] public IPCDQuestionRepository PCDQuestionRepository { get; set; }
        [Inject] public AuthenticationStateProvider Provider { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }

        private bool Sql { get; set; }

        private bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            // Populate RecordUpdatedBy with name of current user.
            var authenticationState = await Provider.GetAuthenticationStateAsync();

            Question.RecordUpdatedBy = authenticationState.User.Claims.Where(x => x.Type == "name").FirstOrDefault().Value ?? "";

            // Show Sql comment box if IsSQLQuery is set to true.
            Sql = Question.IsSQLQuery.ToString() != "True";
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
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
        /// Send put request to server with edited question.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateQuestion()
        {
            // Confirm whether user wants to edit question.
            var confirmation = await Swal.FireAsync(new SweetAlertOptions { Title = "Are you sure you want to edit this question?", Icon = SweetAlertIcon.Info, ShowCancelButton = true });

            if (confirmation.IsConfirmed)
            {
                IsLoading = true;

                try
                {
                    // Get format id.
                    var formatId = await PCDQuestionRepository.GetFormatIdAsync(Question.QuestionFormat);

                    // Generate new PCDQuestion with format id.
                    PCDQuestion pcdQuestion = new() { Id = Question.Id, Index = Question.Index, Question = Question.Question, QuestionFormatId = formatId, IsClientQuestion = Convert.ToBoolean(Question.IsClientQuestion), IsGenericQuestion = Convert.ToBoolean(Question.IsGenericQuestion), IsSQLQuery = Convert.ToBoolean(Question.IsSQLQuery), SQLQuery = Question.SQLQuery, HasFurtherComment = Convert.ToBoolean(Question.HasFurtherComment), RecordUpdatedBy = Question.RecordUpdatedBy };

                    // Update question.
                    await PCDQuestionRepository.UpdateQuestionAsync(pcdQuestion);

                    // Once updated, close modal.
                    await ModalInstance.CloseAsync(ModalResult.Ok(Question.Question));
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    await Swal.FireAsync("Unable to Save Changes", "Please try again or contact I.T. support.", "error");
                }
                finally
                {
                    IsLoading = false;
                    StateHasChanged();
                }
            }
        }
    }
}
