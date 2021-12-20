using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using ModelServices.Interfaces.Repositories;
using ApplicationServices.Services;
using ModelServices.EntitiesServices;
using DataServices.Repositories;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.Start.NinjectWebCommons), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Presentation.Start.NinjectWebCommons), "Stop")]

namespace Presentation.Start
{
    public class NinjectWebCommons
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<IUsuarioAppService>().To<UsuarioAppService>();
            kernel.Bind<ILogAppService>().To<LogAppService>();
            kernel.Bind<IConfiguracaoAppService>().To<ConfiguracaoAppService>();
            kernel.Bind<INotificacaoAppService>().To<NotificacaoAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<IAssinanteAppService>().To<AssinanteAppService>();
            kernel.Bind<IClienteAppService>().To<ClienteAppService>();
            kernel.Bind<IClienteCnpjAppService>().To<ClienteCnpjAppService>();
            kernel.Bind<IMensagemAppService>().To<MensagemAppService>();
            kernel.Bind<IGrupoAppService>().To<GrupoAppService>();
            kernel.Bind<ICategoriaClienteAppService>().To<CategoriaClienteAppService>();
            kernel.Bind<IPosicaoAppService>().To<PosicaoAppService>();
            kernel.Bind<IEMailAgendaAppService>().To<EMailAgendaAppService>();
            kernel.Bind<ICRMAppService>().To<CRMAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<IPlanoAppService>().To<PlanoAppService>();
            kernel.Bind<IAssinanteCnpjAppService>().To<AssinanteCnpjAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<INotificacaoService>().To<NotificacaoService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<IAssinanteService>().To<AssinanteService>();
            kernel.Bind<IClienteService>().To<ClienteService>();
            kernel.Bind<IClienteCnpjService>().To<ClienteCnpjService>();
            kernel.Bind<IMensagemService>().To<MensagemService>();
            kernel.Bind<IGrupoService>().To<GrupoService>();
            kernel.Bind<ICategoriaClienteService>().To<CategoriaClienteService>();
            kernel.Bind<IPosicaoService>().To<PosicaoService>();
            kernel.Bind<IEMailAgendaService>().To<EmailAgendaService>();
            kernel.Bind<ICRMService>().To<CRMService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<IPlanoService>().To<PlanoService>();
            kernel.Bind<IAssinanteCnpjService>().To<AssinanteCnpjService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<ICategoriaNotificacaoRepository>().To<CategoriaNotificacaoRepository>();
            kernel.Bind<INotificacaoRepository>().To<NotificacaoRepository>();
            kernel.Bind<INotificacaoAnexoRepository>().To<NotificacaoAnexoRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IAssinanteRepository>().To<AssinanteRepository>();
            kernel.Bind<IAssinanteAnexoRepository>().To<AssinanteAnexoRepository>();
            kernel.Bind<ICategoriaClienteRepository>().To<CategoriaClienteRepository>();
            kernel.Bind<IClienteRepository>().To<ClienteRepository>();
            kernel.Bind<IClienteAnexoRepository>().To<ClienteAnexoRepository>();
            kernel.Bind<IClienteContatoRepository>().To<ClienteContatoRepository>();
            kernel.Bind<IClienteCnpjRepository>().To<ClienteCnpjRepository>();
            kernel.Bind<ICargoRepository>().To<CargoRepository>();
            kernel.Bind<IMensagemRepository>().To<MensagemRepository>();
            kernel.Bind<IMensagemDestinoRepository>().To<MensagemDestinoRepository>();
            kernel.Bind<IMensagemAnexoRepository>().To<MensagemAnexoRepository>();
            kernel.Bind<IGrupoRepository>().To<GrupoRepository>();
            kernel.Bind<IGrupoContatoRepository>().To<GrupoContatoRepository>();
            kernel.Bind<IPosicaoRepository>().To<PosicaoRepository>();
            kernel.Bind<IEmailAgendaRepository>().To<EMailAgendaRepository>();
            kernel.Bind<ICRMRepository>().To<CRMRepository>();
            kernel.Bind<ICRMAnexoRepository>().To<CRMAnexoRepository>();
            kernel.Bind<ICRMComentarioRepository>().To<CRMComentarioRepository>();
            kernel.Bind<ITipoCRMRepository>().To<TipoCRMRepository>();
            kernel.Bind<ITipoAcaoRepository>().To<TipoAcaoRepository>();
            kernel.Bind<IMotivoCancelamentoRepository>().To<MotivoCancelamentoRepository>();
            kernel.Bind<IMotivoEncerramentoRepository>().To<MotivoEncerramentoRepository>();
            kernel.Bind<ICRMOrigemRepository>().To<CRMOrigemRepository>();
            kernel.Bind<ICRMContatoRepository>().To<CRMContatoRepository>();
            kernel.Bind<ICRMAcaoRepository>().To<CRMAcaoRepository>();
            kernel.Bind<ICategoriaAgendaRepository>().To<CategoriaAgendaRepository>();
            kernel.Bind<IAgendaAnexoRepository>().To<AgendaAnexoRepository>();
            kernel.Bind<IAgendaRepository>().To<AgendaRepository>();
            kernel.Bind<IPlanoRepository>().To<PlanoRepository>();
            kernel.Bind<IPeriodicidadePlanoRepository>().To<PeriodicidadePlanoRepository>();
            kernel.Bind<IAssinantePagamentoRepository>().To<AssinantePagamentoRepository>();
            kernel.Bind<IAssinanteCnpjRepository>().To<AssinanteCnpjRepository>();

        }
    }
}