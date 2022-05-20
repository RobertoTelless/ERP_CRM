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
            kernel.Bind<IEMailAgendaAppService>().To<EMailAgendaAppService>();
            kernel.Bind<ICRMAppService>().To<CRMAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<IPlanoAppService>().To<PlanoAppService>();
            kernel.Bind<IAssinanteCnpjAppService>().To<AssinanteCnpjAppService>();
            kernel.Bind<IFornecedorAppService>().To<FornecedorAppService>();
            kernel.Bind<IFornecedorCnpjAppService>().To<FornecedorCnpjAppService>();
            kernel.Bind<ITemplateSMSAppService>().To<TemplateSMSAppService>();
            kernel.Bind<ITemplateEMailAppService>().To<TemplateEMailAppService>();
            kernel.Bind<INoticiaAppService>().To<NoticiaAppService>();
            kernel.Bind<ITarefaAppService>().To<TarefaAppService>();
            kernel.Bind<ITipoPessoaAppService>().To<TipoPessoaAppService>();
            kernel.Bind<ITelefoneAppService>().To<TelefoneAppService>();
            kernel.Bind<ICategoriaTelefoneAppService>().To<CategoriaTelefoneAppService>();
            kernel.Bind<IFilialAppService>().To<FilialAppService>();
            kernel.Bind<ICargoAppService>().To<CargoAppService>();
            kernel.Bind<ICRMOrigemAppService>().To<CRMOrigemAppService>();
            kernel.Bind<IMotivoCancelamentoAppService>().To<MotivoCancelamentoAppService>();
            kernel.Bind<IMotivoEncerramentoAppService>().To<MotivoEncerramentoAppService>();
            kernel.Bind<ITipoAcaoAppService>().To<TipoAcaoAppService>();
            kernel.Bind<IEquipamentoAppService>().To<EquipamentoAppService>();
            kernel.Bind<ICategoriaEquipamentoAppService>().To<CategoriaEquipamentoAppService>();
            kernel.Bind<ICategoriaFornecedorAppService>().To<CategoriaFornecedorAppService>();
            kernel.Bind<ICategoriaProdutoAppService>().To<CategoriaProdutoAppService>();
            kernel.Bind<ISubcategoriaProdutoAppService>().To<SubcategoriaProdutoAppService>();
            kernel.Bind<IUnidadeAppService>().To<UnidadeAppService>();
            kernel.Bind<ITamanhoAppService>().To<TamanhoAppService>();
            kernel.Bind<IProdutoAppService>().To<ProdutoAppService>();
            kernel.Bind<IProdutoEstoqueFilialAppService>().To<ProdutoEstoqueFilialAppService>();
            kernel.Bind<IProdutotabelaPrecoAppService>().To<ProdutoTabelaPrecoAppService>();
            kernel.Bind<IMovimentoEstoqueProdutoAppService>().To<MovimentoEstoqueProdutoAppService>();
            kernel.Bind<IBancoAppService>().To<BancoAppService>();
            kernel.Bind<ICentroCustoAppService>().To<CentroCustoAppService>();
            kernel.Bind<IContaBancariaAppService>().To<ContaBancariaAppService>();
            kernel.Bind<IFormaPagamentoAppService>().To<FormaPagamentoAppService>();
            kernel.Bind<IGrupoCCAppService>().To<GrupoCCAppService>();
            kernel.Bind<ISubgrupoAppService>().To<SubgrupoAppService>();
            kernel.Bind<IContaPagarAppService>().To<ContaPagarAppService>();
            kernel.Bind<IContaPagarParcelaAppService>().To<ContaPagarParcelaAppService>();
            kernel.Bind<IContaPagarRateioAppService>().To<ContaPagarRateioAppService>();
            kernel.Bind<IContaReceberAppService>().To<ContaReceberAppService>();
            kernel.Bind<IContaReceberParcelaAppService>().To<ContaReceberParcelaAppService>();
            kernel.Bind<IContaReceberRateioAppService>().To<ContaReceberRateioAppService>();
            kernel.Bind<IFichaTecnicaAppService>().To<FichaTecnicaAppService>();
            kernel.Bind<ICategoriaServicoAppService>().To<CategoriaServicoAppService>();
            kernel.Bind<IDepartamentoAppService>().To<DepartamentoAppService>();
            kernel.Bind<IServicoAppService>().To<ServicoAppService>();
            kernel.Bind<IServicoTabelaPrecoAppService>().To<ServicoTabelaPrecoAppService>();
            kernel.Bind<ICategoriaAtendimentoAppService>().To<CategoriaAtendimentoAppService>();
            kernel.Bind<IAtendimentoAgendaAppService>().To<AtendimentoAgendaAppService>();
            kernel.Bind<IAtendimentoAppService>().To<AtendimentoAppService>();
            kernel.Bind<IPeriodicidadeAppService>().To<PeriodicidadeAppService>();
            kernel.Bind<IMensagemAutomacaoAppService>().To<MensagemAutomacaoAppService>();
            kernel.Bind<IPedidoCompraAppService>().To<PedidoCompraAppService>();
            kernel.Bind<IOrdemServicoAgendaAppService>().To<OrdemServicoAgendaAppService>();
            kernel.Bind<IOrdemServicoAppService>().To<OrdemServicoAppService>();
            kernel.Bind<IOrdemServicoProdutoAppService>().To<OrdemServicoProdutoAppService>();
            kernel.Bind<IOrdemServicoServicoAppService>().To<OrdemServicoServicoAppService>();
            kernel.Bind<ICategoriaOrdemServicoAppService>().To<CategoriaOrdemServicoAppService>();
            kernel.Bind<ICRMComercialAppService>().To<CRMComercialAppService>();
            kernel.Bind<IFormularioRespostaAppService>().To<FormularioRespostaAppService>();
            kernel.Bind<ITransportadoraAppService>().To<TransportadoraAppService>();

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
            kernel.Bind<IEMailAgendaService>().To<EmailAgendaService>();
            kernel.Bind<ICRMService>().To<CRMService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<IPlanoService>().To<PlanoService>();
            kernel.Bind<IAssinanteCnpjService>().To<AssinanteCnpjService>();
            kernel.Bind<IFornecedorService>().To<FornecedorService>();
            kernel.Bind<IFornecedorCnpjService>().To<FornecedorCnpjService>();
            kernel.Bind<ITemplateSMSService>().To<TemplateSMSService>();
            kernel.Bind<ITemplateEMailService>().To<TemplateEMailService>();
            kernel.Bind<INoticiaService>().To<NoticiaService>();
            kernel.Bind<ITarefaService>().To<TarefaService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<ITelefoneService>().To<TelefoneService>();
            kernel.Bind<ICategoriaTelefoneService>().To<CategoriaTelefoneService>();
            kernel.Bind<IFilialService>().To<FilialService>();
            kernel.Bind<ICargoService>().To<CargoService>();
            kernel.Bind<ICRMOrigemService>().To<CRMOrigemService>();
            kernel.Bind<IMotivoCancelamentoService>().To<MotivoCancelamentoService>();
            kernel.Bind<IMotivoEncerramentoService>().To<MotivoEncerramentoService>();
            kernel.Bind<ITipoAcaoService>().To<TipoAcaoService>();
            kernel.Bind<IEquipamentoService>().To<EquipamentoService>();
            kernel.Bind<ICategoriaEquipamentoService>().To<CategoriaEquipamentoService>();
            kernel.Bind<ICategoriaFornecedorService>().To<CategoriaFornecedorService>();
            kernel.Bind<ICategoriaProdutoService>().To<CategoriaProdutoService>();
            kernel.Bind<ISubcategoriaProdutoService>().To<SubcategoriaProdutoService>();
            kernel.Bind<IUnidadeService>().To<UnidadeService>();
            kernel.Bind<ITamanhoService>().To<TamanhoService>();
            kernel.Bind<IProdutoService>().To<ProdutoService>();
            kernel.Bind<IProdutoEstoqueFilialService>().To<ProdutoEstoqueFilialService>();
            kernel.Bind<IProdutoMovimentoEstoqueService>().To<ProdutoMovimentoEstoqueService>();
            kernel.Bind<IProdutoTabelaPrecoService>().To<ProdutoTabelaPrecoService>();
            kernel.Bind<IMovimentoEstoqueProdutoService>().To<MovimentoEstoqueProdutoService>();
            kernel.Bind<IBancoService>().To<BancoService>();
            kernel.Bind<ICentroCustoService>().To<CentroCustoService>();
            kernel.Bind<IContaBancariaService>().To<ContaBancariaService>();
            kernel.Bind<IFormaPagamentoService>().To<FormaPagamentoService>();
            kernel.Bind<IGrupoCCService>().To<GrupoCCService>();
            kernel.Bind<ISubgrupoService>().To<SubgrupoService>();
            kernel.Bind<IContaPagarService>().To<ContaPagarService>();
            kernel.Bind<IContaPagarParcelaService>().To<ContaPagarParcelaService>();
            kernel.Bind<IContaPagarRateioService>().To<ContaPagarRateioService>();
            kernel.Bind<IContaReceberParcelaService>().To<ContaReceberParcelaService>();
            kernel.Bind<IContaReceberRateioService>().To<ContaReceberRateioService>();
            kernel.Bind<IContaReceberService>().To<ContaReceberService>();
            kernel.Bind<IFichaTecnicaService>().To<FichaTecnicaService>();
            kernel.Bind<ICategoriaServicoService>().To<CategoriaServicoService>();
            kernel.Bind<IDepartamentoService>().To<DepartamentoService>();
            kernel.Bind<IServicoService>().To<ServicoService>();
            kernel.Bind<IServicoTabelaPrecoService>().To<ServicoTabelaPrecoService>();
            kernel.Bind<ICategoriaAtendimentoService>().To<CategoriaAtendimentoService>();
            kernel.Bind<IAtendimentoAgendaService>().To<AtendimentoAgendaService>();
            kernel.Bind<IAtendimentoService>().To<AtendimentoService>();
            kernel.Bind<IPeriodicidadeService>().To<PeriodicidadeService>();
            kernel.Bind<IMensagemAutomacaoService>().To<MensagemAutomacaoService>();
            kernel.Bind<IPedidoCompraService>().To<PedidoCompraService>();
            kernel.Bind<IOrdemServicoAcompanhamentoService>().To<OrdemServicoAcompanhamentoService>();
            kernel.Bind<IOrdemServicoAgendaService>().To<OrdemServicoAgendaService>();
            kernel.Bind<IOrdemServicoComentarioService>().To<OrdemServicoComentarioService>();
            kernel.Bind<IOrdemServicoProdutoService>().To<OrdemServicoProdutoService>();
            kernel.Bind<IOrdemServicoServicoService>().To<OrdemServicoServicoService>();
            kernel.Bind<ICategoriaOrdemServicoService>().To<CategoriaOrdemServicoService>();
            kernel.Bind<IOrdemServicoService>().To<OrdemServicoService>();
            kernel.Bind<ICRMComercialService>().To<CRMComercialService>();
            kernel.Bind<IFormularioRespostaService>().To<FormularioRespostaService>();
            kernel.Bind<ITransportadoraService>().To<TransportadoraService>();

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
            kernel.Bind<ITipoContribuinteRepository>().To<TipoContribuinteRepository>();
            kernel.Bind<IRegimeTributarioRepository>().To<RegimeTributarioRepository>();
            kernel.Bind<ISexoRepository>().To<SexoRepository>();
            kernel.Bind<IFornecedorRepository>().To<FornecedorRepository>();
            kernel.Bind<ICategoriaFornecedorRepository>().To<CategoriaFornecedorRepository>();
            kernel.Bind<IFornecedorContatoRepository>().To<FornecedorContatoRepository>();
            kernel.Bind<IFornecedorAnexoRepository>().To<FornecedorAnexoRepository>();
            kernel.Bind<IFornecedorCnpjRepository>().To<FornecedorCnpjRepository>();
            kernel.Bind<ITemplateSMSRepository>().To<TemplateSMSRepository>();
            kernel.Bind<ITemplateEMailRepository>().To<TemplateEMailRepository>();
            kernel.Bind<INoticiaRepository>().To<NoticiaRepository>();
            kernel.Bind<INoticiaComentarioRepository>().To<NoticiaComentarioRepository>();
            kernel.Bind<ITarefaRepository>().To<TarefaRepository>();
            kernel.Bind<ITarefaAnexoRepository>().To<TarefaAnexoRepository>();
            kernel.Bind<ITarefaNotificacaoRepository>().To<TarefaNotificacaoRepository>();
            kernel.Bind<ITipoTarefaRepository>().To<TipoTarefaRepository>();
            kernel.Bind<ITelefoneRepository>().To<TelefoneRepository>();
            kernel.Bind<ICategoriaTelefoneRepository>().To<CategoriaTelefoneRepository>();
            kernel.Bind<IFilialRepository>().To<FilialRepository>();
            kernel.Bind<IClienteTagRepository>().To<ClienteTagRepository>();
            kernel.Bind<IClienteReferenciaRepository>().To<ClienteReferenciaRepository>();
            kernel.Bind<IEquipamentoAnexoRepository>().To<EquipamentoAnexoRepository>();
            kernel.Bind<IEquipamentoManutencaoRepository>().To<EquipamentoManutencaoRepository>();
            kernel.Bind<IEquipamentoRepository>().To<EquipamentoRepository>();
            kernel.Bind<ICategoriaEquipamentoRepository>().To<CategoriaEquipamentoRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<ICategoriaProdutoRepository>().To<CategoriaProdutoRepository>();
            kernel.Bind<ISubcategoriaProdutoRepository>().To<SubcategoriaProdutoRepository>();
            kernel.Bind<IUnidadeRepository>().To<UnidadeRepository>();
            kernel.Bind<ITamanhoRepository>().To<TamanhoRepository>();
            kernel.Bind<IProdutoAnexoRepository>().To<ProdutoAnexoRepository>();
            kernel.Bind<IProdutoBarcodeRepository>().To<ProdutoBarcodeRepository>();
            kernel.Bind<IProdutoEstoqueFilialRepository>().To<ProdutoEstoqueFilialRepository>();
            kernel.Bind<IProdutoFornecedorRepository>().To<ProdutoFornecedorRepository>();
            kernel.Bind<IProdutoGradeRepository>().To<ProdutoGradeRepository>();
            kernel.Bind<IProdutoMovimentoEstoqueRepository>().To<ProdutoMovimentoEstoqueRepository>();
            kernel.Bind<IProdutoOrigemRepository>().To<ProdutoOrigemRepository>();
            kernel.Bind<IProdutoRepository>().To<ProdutoRepository>();
            kernel.Bind<IProdutoTabelaPrecoRepository>().To<ProdutoTabelaPrecoRepository>();
            kernel.Bind<IMovimentoEstoqueProdutoRepository>().To<MovimentoEstoqueProdutoRepository>();
            kernel.Bind<IBancoRepository>().To<BancoRepository>();
            kernel.Bind<ICentroCustoRepository>().To<CentroCustoRepository>();
            kernel.Bind<IContaBancariaRepository>().To<ContaBancariaRepository>();
            kernel.Bind<IContaBancariaContatoRepository>().To<ContaBancariaContatoRepository>();
            kernel.Bind<IContaBancariaLancamentoRepository>().To<ContaBancariaLancamentoRepository>();
            kernel.Bind<ITipoContaRepository>().To<TipoContaRepository>();
            kernel.Bind<IFormaPagamentoRepository>().To<FormaPagamentoRepository>();
            kernel.Bind<IItemPedidoCompraRepository>().To<ItemPedidoCompraRepository>();
            kernel.Bind<IGrupoCCRepository>().To<GrupoCCRepository>();
            kernel.Bind<ISubgrupoRepository>().To<SubgrupoRepository>();
            kernel.Bind<IContaPagarAnexoRepository>().To<ContaPagarAnexoRepository>();
            kernel.Bind<IContaPagarParcelaRepository>().To<ContaPagarParcelaRepository>();
            kernel.Bind<IContaPagarRateioRepository>().To<ContaPagarRateioRepository>();
            kernel.Bind<IContaPagarRepository>().To<ContaPagarRepository>();
            kernel.Bind<IContaReceberAnexoRepository>().To<ContaReceberAnexoRepository>();
            kernel.Bind<IContaReceberParcelaRepository>().To<ContaReceberParcelaRepository>();
            kernel.Bind<IContaReceberRateioRepository>().To<ContaReceberRateioRepository>();
            kernel.Bind<IContaReceberRepository>().To<ContaReceberRepository>();
            kernel.Bind<IFichaTecnicaRepository>().To<FichaTecnicaRepository>();
            kernel.Bind<IFichaTecnicaDetalheRepository>().To<FichaTecnicaDetalheRepository>();
            kernel.Bind<IProdutoKitRepository>().To<ProdutoKitRepository>();
            kernel.Bind<ICategoriaServicoRepository>().To<CategoriaServicoRepository>();
            kernel.Bind<IDepartamentoRepository>().To<DepartamentoRepository>();
            kernel.Bind<IServicoAnexoRepository>().To<ServicoAnexoRepository>();
            kernel.Bind<IServicoRepository>().To<ServicoRepository>();
            kernel.Bind<IServicoTabelaPrecoRepository>().To<ServicoTabelaPrecoRepository>();
            kernel.Bind<ICategoriaAtendimentoRepository>().To<CategoriaAtendimentoRepository>();
            kernel.Bind<IAtendimentoAgendaRepository>().To<AtendimentoAgendaRepository>();
            kernel.Bind<IAtendimentoAnexoRepository>().To<AtendimentoAnexoRepository>();
            kernel.Bind<IAtendimentoRepository>().To<AtendimentoRepository>();
            kernel.Bind<INomencBrasServicosRepository>().To<NomencBrasServicosRepository>();
            kernel.Bind<IMensagemAutomacaoRepository>().To<MensagemAutomacaoRepository>();
            kernel.Bind<IMensagemAutomacaoDatasRepository>().To<MensagemAutomacaoDatasRepository>();
            kernel.Bind<IPedidoCompraRepository>().To<PedidoCompraRepository>();
            kernel.Bind<IPedidoCompraAnexoRepository>().To<PedidoCompraAnexoRepository>();
            kernel.Bind<IPedidoCompraAcompanhamentoRepository>().To<PedidoCompraAcompanhamentoRepository>();
            kernel.Bind<IOrdemServicoAcompanhamentoRepository>().To<OrdemServicoAcompanhamentoRepository>();
            kernel.Bind<IOrdemServicoAgendaRepository>().To<OrdemServicoAgendaRepository>();
            kernel.Bind<IOrdemServicoAnexoRepository>().To<OrdemServicoAnexoRepository>();
            kernel.Bind<IOrdemServicoComentarioRepository>().To<OrdemServicoComentarioRepository>();
            kernel.Bind<IOrdemServicoProdutoRepository>().To<OrdemServicoProdutoRepository>();
            kernel.Bind<IOrdemServicoRepository>().To<OrdemServicoRepository>();
            kernel.Bind<IOrdemServicoServicoRepository>().To<OrdemServicoServicoRepository>();
            kernel.Bind<ICategoriaOrdemServicoRepository>().To<CategoriaOrdemServicoRepository>();
            kernel.Bind<ICRMComercialAcaoRepository>().To<CRMComercialAcaoRepository>();
            kernel.Bind<ICRMComercialAnexoRepository>().To<CRMComercialAnexoRepository>();
            kernel.Bind<ICRMComercialComentarioRepository>().To<CRMComercialComentarioRepository>();
            kernel.Bind<ICRMComercialContatoRepository>().To<CRMComercialContatoRepository>();
            kernel.Bind<ICRMComercialItemRepository>().To<CRMComercialItemRepository>();
            kernel.Bind<ICRMComercialRepository>().To<CRMComercialRepository>();
            kernel.Bind<IFormularioRespostaRepository>().To<FormularioRespostaRepository>();
            kernel.Bind<IFormularioRespostaAcaoRepository>().To<FormularioRespostaAcaoRepository>();
            kernel.Bind<IFormularioRespostaAnexoRepository>().To<FormularioRespostaAnexoRepository>();
            kernel.Bind<IFormularioRespostaComentarioRepository>().To<FormularioRespostaComentarioRepository>();
            kernel.Bind<ITemplatePropostaRepository>().To<TemplatePropostaRepository>();
            kernel.Bind<ICRMPropostaRepository>().To<CRMPropostaRepository>();
            kernel.Bind<ICRMPropostaComentarioRepository>().To<CRMPropostaComentarioRepository>();
            kernel.Bind<ICRMPropostaAnexoRepository>().To<CRMPropostaAnexoRepository>();
            kernel.Bind<ICRMPedidoRepository>().To<CRMPedidoRepository>();
            kernel.Bind<ICRMPedidoAnexoRepository>().To<CRMPedidoAnexoRepository>();
            kernel.Bind<ICRMPedidoComentarioRepository>().To<CRMPedidoComentarioRepository>();
            kernel.Bind<ICRMItemPedidoRepository>().To<CRMItemPedidoRepository>();
            kernel.Bind<IFormaEnvioRepository>().To<FormaEnvioRepository>();
            kernel.Bind<IFormaFreteRepository>().To<FormaFreteRepository>();
            kernel.Bind<ITransportadoraRepository>().To<TransportadoraRepository>();
            kernel.Bind<ITransportadoraAnexoRepository>().To<TransportadoraAnexoRepository>();
            kernel.Bind<ITipoVeiculoRepository>().To<TipoVeiculoRepository>();
            kernel.Bind<ITipoTransporteRepository>().To<TipoTransporteRepository>();


        }
    }
}