using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Elsa;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.SqlServer;
using Solutions.Now.Moe.Elsa.Activities;
using Elsa.Scripting.Liquid.Messages;
using Solutions.Now.Moe.Elsa.Handlers;
using Solutions.Now.Moe.Elsa.Activities.Construction;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using Solutions.Now.Moe.Elsa.Models.Construction;
using Automatonymous;
using Quartz;
using Solutions.Now.CMIS2.Elsa.Activities;

namespace Solutions.Now.Moe.Elsa
{
    public class Startup
    { 
         //test Push fgfgfg
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            var elsaSection = Configuration.GetSection("Elsa");

            // Elsa services.
           services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),false)
                    .AddConsoleActivities()
                    .AddActivity<NotifictionInterval>()
                    .AddActivity<PlanningWorkFlow>()
                    .AddActivity<AddApproval>()
                    .AddActivity<BookApproval>()
                    .AddActivity<CommitteeApproval>()
                    .AddActivity<CommitteeMemberApproval>()
                    .AddActivity<SiteVisitApproval>()
                    .AddActivity<ProjectCancellationApproval>()
                    .AddActivity<TenderApproval>()
                    .AddActivity<AssignedEmployee>()
                    .AddActivity<VisitMemberApproval>()
                    .AddActivity<SiteVisitApprovalConsultant>()
                    .AddActivity<projectStop>()
                    .AddActivity<CommitteeStop>()
                    .AddActivity<CommitteeCancellation>()
                    .AddActivity<ResumeProject>()
                    .AddActivity<CommitteeResume>()
                    .AddActivity<Financialpayment>()
                    .AddActivity<ApproveAcountantuser>()
                    .AddActivity<projectsTenderApproval>()  
                    .AddActivity<projectsTenderFlagsApproval>()
                    .AddActivity<CommitteeStudyDesignProject>()
                    .AddActivity<WorkflowProjectDesign>()
                    .AddActivity<getArchitecturalPainter>()
                    .AddActivity<getQuantitativeCalculator>()
                    .AddActivity<AddApprovalForCommitteMember>()
                    .AddActivity<CommitteeMembersList>()
                    .AddActivity<siteVisitDateToNotificate>()
                    .AddActivity<Construction_AddApproval>()
                    .AddActivity<Construction_AddTenderUsers>()
                    .AddActivity<Construction_CommitteeMemberUsers>()
                    .AddActivity<Construction_NotifictionInterval>()
                    .AddActivity<Construction_CommunicationEngUsers>()
                    .AddActivity<Construction_delegateDirectorEducationWithEngineerPermissionsUsers>()
                    .AddActivity<Construction_SupervisionCommitteeUsers>()
                    .AddActivity<Construction_SiteHandOverUsers>()
                    .AddActivity<Construction_siteHandOverDateToNotificate>()
                    .AddActivity<Construction_DirectOrderToTheContractorUsers>()
                    .AddActivity<Construction_RaiseSurveyorsUsers>()                    
                    .AddActivity<Construction_InsurancePolicyUsers>()
                    .AddActivity<Construction_ContractorStaffUsers>()       
                    .AddActivity<Construction_LaboratoryAccreditation>()
                    .AddActivity<Construction_Adoptionofmaterials>()
                    .AddActivity<Activities.Construction_AdoptionMandatoryOperatingContractor>()
                    .AddActivity<Activities.Construction_Downpayment>()
                    .AddActivity<Construction_WorkSchedule> ()
                    .AddActivity< Construction_ModifiedWorkSchedule> ()
                    .AddActivity<Construction_Formationofacommittee>()
                    .AddActivity<Construction_Paymentforcompletion>()
                    .AddActivity<Construction_InterimPaymentForConstructorUsers>()
                    .AddActivity<Construction_ClosingPaymentUsers>()
                    .AddActivity<MixedDesignUsers>()
                    .AddActivity<Construction_ContractorFinancialTemporalClaimsUsers>()
                    .AddActivity<Construction_CaptainCommitteeMemberUsers>()
                    .AddActivity<Construction_GetConcernUserMoeUsers>()
                     .AddActivity<Construction_ChangeOrderUsers>()
                    .AddActivity<changeOrderType>()
                    .AddActivity<Construction_InitialReceipt>()
                    .AddActivity<Construction_partial_Receipt_Works>()
                    .AddActivity<Construction_CountCommitteeUsers>()
                    .AddActivity<Construction_ContractorMandatoryStaffUsers>()
                    .AddActivity<Construction_ReleaseReservations>()
                    .AddActivity<Construction_finalDelivery>()
                    .AddActivity<ConstructionSendRequestWorkflowUsers>()
                    .AddActivity<Construction_Non_complianceWithActionsCorrectiveActions>()
                    .AddActivity<Construction_OfficialCommunicationEngineerBooks>()
                    .AddActivity<construcion_HeadOfficialBooksDepartment>()
                           .AddActivity<Construction_SiteNote>()
                    .AddActivity<Construction_DailyWorkflow>()
                    .AddActivity<Construction_ApprovalBooksByContractor>()

                    .AddActivity<Construction_SiteNote>()
                    .AddActivity<Construction_DailyWorkflow>()
                    .AddActivity<Construction_ApprovalBooksByContractor>()
                            .AddActivity<Construction_DefectsLiabilityContractorDurationInitialReceipt>()
                    .AddActivity<Construction_DefectsLiabilityContractorDurationFinal>()
                    .AddActivity<Construction_DefectsLiabilityContractorDurationPartial>()
       


                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddQuartzTemporalActivities()
                    .AddWorkflowsFrom<Startup>()
                   
                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();
            services.Configure<ApiVersioningOptions>(options => options.UseApiBehavior = false);

            // Allow arbitrary client browser apps to access the API.
            // In a production environment, make sure to allow only origins you trust.
            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders("Content-Disposition"))
            );

            services.AddDbContext<MoeDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionMoe")));
            services.AddDbContext<SsoDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionSSO")));
            services.AddDbContext<DesignReviewDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionDesignReview")));
            services.AddDbContext<ConstructionDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionMoe")));

            services.AddNotificationHandler<EvaluatingLiquidExpression, ConfigureLiquidEngine>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHttpActivities();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
