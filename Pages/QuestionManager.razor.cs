using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using SurveyAdmin.Components;
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
    public partial class QuestionManager
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Inject] public IPCDQuestionRepository PCDQuestionRepository { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }

        private List<PCDQuestionDto> Questions;

        protected override async Task OnInitializedAsync()
        {
            // Get list of all questions.
            Questions = await GetAllQuestionsAsync();
        }

        private async Task NewQuestion()
        {
            var options = new ModalOptions()
            {
                Animation = ModalAnimation.FadeIn(1)
            };

            var questionModal = Modal.Show<QuestionNew>("New Question", options);
            var result = await questionModal.Result;

            if (!result.Cancelled)
            {
                // Refresh list of questions.
                Questions = await GetAllQuestionsAsync();

                await Swal.FireAsync("Question Added", $"\"{result.Data}\" has been added.", "success");
            }
        }

        private async Task EditQuestion(PCDQuestionDto question)
        {
            var parameters = new ModalParameters();
            parameters.Add("Question", question);

            var options = new ModalOptions()
            {
                Animation = ModalAnimation.FadeIn(1)
            };

            var questionModal = Modal.Show<QuestionEdit>("Edit Question", parameters, options);
            var result = await questionModal.Result;

            if (!result.Cancelled)
            {
                // Refresh list of questions.
                Questions = await GetAllQuestionsAsync();

                await Swal.FireAsync("Question Updated", $"Your changes to question \"{result.Data}\" have been saved", "success");
            }
        }

        private async Task QuestionDetail(PCDQuestionDto question)
        {
            var parameters = new ModalParameters();
            parameters.Add("Question", question);

            var options = new ModalOptions()
            {
                Animation = ModalAnimation.FadeIn(1)
            };

            var questionModal = Modal.Show<QuestionDetail>("Question Detail", parameters, options);
            var result = await questionModal.Result;

            if (!result.Cancelled)
            {
                // Refresh list of questions.
                Questions = await GetAllQuestionsAsync();

                await Swal.FireAsync("Question Deleted", $"\"{result.Data}\" has been deleted.", "info");
            }
        }
                
        private async Task<List<PCDQuestionDto>> GetAllQuestionsAsync()
        {
            try
            {
                return (await PCDQuestionRepository.GetPCDQuestionsAsync()).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Load Questions", "Please try again or contact I.T. support.", "error");
                return default;
            }
        }
    }
}
