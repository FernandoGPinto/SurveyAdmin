using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
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
    public partial class QuestionDetail
    {
        [CascadingParameter] public BlazoredModalInstance ModalInstance { get; set; }
        [Parameter] public PCDQuestionDto Question { get; set; }
        [Inject] public IPCDQuestionRepository PCDQuestionRepository { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }

        private bool Sql { get; set; }

        protected override void OnInitialized()
        {
            // Show Sql comment box if IsSQLQuery is set to true.
            Sql = Question.IsSQLQuery.ToString() != "True";
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }

        /// <summary>
        /// Send delete request to server.
        /// </summary>
        /// <returns></returns>
        private async Task DeleteQuestion()
        {
            // Confirm whether user wants to delete question.
            var confirmation = await Swal.FireAsync(new SweetAlertOptions { Title = "Are you sure?", Text = $"You are about to delete \"{Question.Question}\".", Icon = SweetAlertIcon.Warning, ShowCancelButton = true });

            if (confirmation.IsConfirmed)
            {
                try
                {
                    // Delete question.
                    await PCDQuestionRepository.DeleteQuestionAsync(Question.Id);

                    // Once deleted, close modal.
                    await ModalInstance.CloseAsync(ModalResult.Ok(Question.Question));
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    await Swal.FireAsync("Unable to Delete Question", "Please try again or contact I.T. support.", "error");
                }
            }
        }
    }
}
