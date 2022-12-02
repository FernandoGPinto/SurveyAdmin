using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SurveyAdmin.Data;
using SurveyAdmin.Interfaces;
using SurveyAdmin.Models;
using SurveyAdmin.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin.Pages
{
    public partial class Dashboard
    {
        [CascadingParameter] public IModalService Modal { get; set; }
        [Inject] public IPCDSurveyResponseRepository PCDSurveyResponseRepository { get; set; }
        [Inject] public IClientRepository ClientRepository { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private readonly PCDSearchParameters SearchParameters = new();

        private List<Client> Clients;
        private List<ViewPCDResponses> Responses;

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

        private async Task Search()
        {
            IsLoading = true;

            try
            {
                Responses = (await PCDSurveyResponseRepository.GetSubmittedPCDsAsync(SearchParameters.ClientId, SearchParameters.DateFrom, SearchParameters.DateTo)).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Load PCDs", "An error occurred while trying to load the PCD list. Please try again or contact I.T. Support.", "error");
            }
            finally
            {
                IsLoading = false;
                // Trigger state has changed to ensure loading spinner is removed.
                StateHasChanged();
                await JSRuntime.InvokeAsync<object>("AddDataTable", "#PCDTable");
            }
        }

        private async Task Export(ViewPCDResponses response)
        {
            try
            {
                var pcdResponses = (await PCDSurveyResponseRepository.GetPCDDetailsAsync(response.Id)).ToList();

                // Export to Excel.
                var excelExport = new ExcelExport("PCD", ExcelTemplate.Default);
                var stream = excelExport.PCDToExcel(pcdResponses);
                var bytes = stream.ToArray();
                stream.Close();

                await JSRuntime.InvokeAsync<object>("SaveAsFile", $"PCD_{response.Name}_{response.JobNumber}.xlsx", "application/vnd.ms-excel", bytes);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Export to Excel", "An error occurred while trying to export the Excel file. Please try again or contact I.T. Support.", "error");
            }
        }
    }
}
