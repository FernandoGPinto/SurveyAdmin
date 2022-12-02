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

namespace SurveyAdmin.Pages
{
    public partial class SurveyManager
    {
        [Inject] public IPCDQuestionRepository PCDQuestionRepository { get; set; }
        [Inject] public IClientRepository ClientRepository { get; set; }
        [Inject] public IPCDQuestionsClientsRepository PCDQuestionsClientsRepository { get; set; }
        [Inject] public UnitOfWork UnitOfWork { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }

        private List<Client> Clients;

        private List<PCDQuestionAssignmentDto> PreviousQuestionAssignments;

        private List<PCDQuestionAssignmentDto> QuestionAssignments;

        private string SelectedClient;

        private bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Get list of all clients.
                Clients = (await ClientRepository.GetClientsAsync()).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Load Clients", "An error occurred while trying to load the client list. Please try again or contact I.T. Support.", "error");
            }
        }

        /// <summary>
        /// Requests and displays the list of questions relevant to this client.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task ShowQuestionList(ChangeEventArgs e)
        {
            IsLoading = true;

            try
            {
                SelectedClient = e.Value.ToString();

                if (!String.IsNullOrWhiteSpace(SelectedClient))
                {
                    // Get list of all non-generic questions.
                    QuestionAssignments = (await PCDQuestionRepository.GetNonGenericPCDQuestionsAsync(SelectedClient)).ToList();

                    IsLoading = false;

                    if (QuestionAssignments != null)
                    {
                        PreviousQuestionAssignments = new List<PCDQuestionAssignmentDto>();
                        QuestionAssignments.ForEach((item) =>
                        {
                            PreviousQuestionAssignments.Add(new PCDQuestionAssignmentDto(item));
                        });
                    }
                }
                else
                {
                    QuestionAssignments = null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                await Swal.FireAsync("Unable to Load Questions", "An error occurred while trying to load the question list. Please try again or contact I.T. Support.", "error");
            }
            finally
            {
                IsLoading = false;
                // Trigger state has changed to ensure loading spinner is removed.
                StateHasChanged();
            }
        }

        private void AssignQuestion(PCDQuestionAssignmentDto question)
        {
            question.Assigned = question.Assigned ? false : true;
        }

        /// <summary>
        /// Saves changes to question assignments.
        /// </summary>
        /// <returns>Task</returns>
        private async Task SaveChanges()
        {
            IsLoading = true;

            // Create new list of assignments that represents changes from the initial list. Only the changes will be sent to the server for deleting or inserting.
            // Do not remove questions from the list displayed to the user.
            var NewQuestionAssignments = QuestionAssignments.ToList();

            NewQuestionAssignments.RemoveAll(x => PreviousQuestionAssignments.Select(s => new { s.QuestionId, s.Assigned }).Contains(new { x.QuestionId, x.Assigned }));

            await SaveAssignmentsAsync(NewQuestionAssignments, SelectedClient);

            IsLoading = false;

            // Trigger component re-rendering at this stage to ensure that the loading spinner is removed before the swal modal pop up. ComponentBase cannot re-render until the modal is closed and the modal cannot be closed while the loading spinner is on.
            StateHasChanged();

            await Swal.FireAsync("Changes Saved", "The questions assigned to the selected client have been updated.", "success");

            // Update PreviousQuestionAssignments list to reflect the changes made.
            PreviousQuestionAssignments = new List<PCDQuestionAssignmentDto>();
            QuestionAssignments.ForEach((item) =>
            {
                PreviousQuestionAssignments.Add(new PCDQuestionAssignmentDto(item));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionAssignments"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private async Task SaveAssignmentsAsync(List<PCDQuestionAssignmentDto> questionAssignments, string clientId)
        {
            // Create arrays with records to be inserted and deleted.
            var inserts = questionAssignments.Where(x => x.Assigned).Select(s => new PCDQuestionsClients { QuestionId = s.QuestionId, ClientId = Guid.Parse(clientId) }).ToArray();

            var deletes = questionAssignments.Where(x => !x.Assigned).Select(s => new PCDQuestionsClients { QuestionId = s.QuestionId, ClientId = Guid.Parse(clientId) }).ToArray();

            try
            {
                // Begin transaction and rollback all changes if any of the inserts or deletes fails.
                UnitOfWork.BeginTransaction();
                await PCDQuestionsClientsRepository.InsertQuestionAssignments(inserts);
                await PCDQuestionsClientsRepository.DeleteQuestionAssignments(deletes);
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
