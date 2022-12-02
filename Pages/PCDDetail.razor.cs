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
    public partial class PCDDetail
    {
        [Inject] public IPCDSurveyResponseRepository PCDSurveyResponseRepository { get; set; }
        [Inject] public SweetAlertService Swal { get; set; }
        [Parameter] public string JobId { get; set; }
        [Parameter] public string JobNumber { get; set; }
        [Parameter] public string Name { get; set; }
        [Parameter] public string SiteName { get; set; }

        private List<ViewPCDResponse> Responses;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Responses = (await PCDSurveyResponseRepository.GetPCDDetailsAsync(Guid.Parse(JobId))).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                await Swal.FireAsync("Unable to Load PCD Details", "An error occurred while trying to load the PCD details. Please try again or contact I.T. support.", "error");
            }
        }
    }
}
