using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_CRM_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<NotificacaoViewModel, NOTIFICACAO>();
            CreateMap<MensagemViewModel, MENSAGENS>();
            CreateMap<GrupoViewModel, GRUPO>();
            CreateMap<GrupoContatoViewModel, GRUPO_CLIENTE>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<CRMViewModel, CRM>();
            CreateMap<CRMContatoViewModel, CRM_CONTATO>();
            CreateMap<CRMComentarioViewModel, CRM_COMENTARIO>();
            CreateMap<CRMAcaoViewModel, CRM_ACAO>();
            CreateMap<AgendaViewModel, AGENDA>();
            CreateMap<PlanoViewModel, PLANO>();
            CreateMap<AssinanteViewModel, ASSINANTE>();
            CreateMap<AssinantePagamentoViewModel, ASSINANTE_PAGAMENTO>();
            CreateMap<FilialViewModel, FILIAL>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<TelefoneViewModel, TELEFONE>();
            CreateMap<TarefaViewModel, TAREFA>();
            CreateMap<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>();
            CreateMap<ClienteViewModel, CLIENTE>();
            CreateMap<ClienteContatoViewModel, CLIENTE_CONTATO>();
            CreateMap<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>();
            CreateMap<ClienteTagViewModel, CLIENTE_TAG>();
            CreateMap<FornecedorViewModel, FORNECEDOR>();
            CreateMap<FornecedorContatoViewModel, FORNECEDOR_CONTATO>();
            CreateMap<FornecedorComentarioViewModel, FORNECEDOR_COMENTARIO>();
            CreateMap<TemplateSMSViewModel, TEMPLATE_SMS>();
            CreateMap<TemplateEMailViewModel, TEMPLATE_EMAIL>();
            CreateMap<CategoriaClienteViewModel, CATEGORIA_CLIENTE>();
            CreateMap<CargoViewModel, CARGO>();
            CreateMap<CRMOrigemViewModel, CRM_ORIGEM>();
            CreateMap<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>();
            CreateMap<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>();
            CreateMap<TipoAcaoViewModel, TIPO_ACAO>();
            CreateMap<EquipamentoViewModel, EQUIPAMENTO>();
            CreateMap<EquipamentoManutencaoViewModel, EQUIPAMENTO_MANUTENCAO>();
            CreateMap<CategoriaEquipamentoViewModel, CATEGORIA_EQUIPAMENTO>();
            CreateMap<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>();
            CreateMap<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>();
            CreateMap<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>();
            CreateMap<UnidadeViewModel, UNIDADE>();
            CreateMap<TamanhoViewModel, TAMANHO>();
            CreateMap<ProdutoBarcodeViewModel, PRODUTO_BARCODE>();
            CreateMap<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>();
            CreateMap<ProdutoGradeViewModel, PRODUTO_GRADE>();
            CreateMap<ProdutoTabelaPrecoViewModel, PRODUTO_TABELA_PRECO>();
            CreateMap<ProdutoViewModel, PRODUTO>();
            CreateMap<BancoViewModel, BANCO>();
            CreateMap<CentroCustoViewModel, CENTRO_CUSTO>();
            CreateMap<ContaBancariaViewModel, CONTA_BANCO>();
            CreateMap<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>();
            CreateMap<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>();
            CreateMap<FormaPagamentoViewModel, FORMA_PAGAMENTO>();
            CreateMap<ContaPagarParcelaViewModel, CONTA_PAGAR_PARCELA>();
            CreateMap<ContaPagarRateioViewModel, CONTA_PAGAR_RATEIO>();
            CreateMap<ContaPagarViewModel, CONTA_PAGAR>();
            CreateMap<ContaReceberParcelaViewModel, CONTA_RECEBER_PARCELA>();
            CreateMap<ContaReceberViewModel, CONTA_RECEBER>();
            CreateMap<FichaTecnicaViewModel, FICHA_TECNICA>();
            CreateMap<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>();
            CreateMap<ProdutoKitViewModel, PRODUTO_KIT>();
            CreateMap<CategoriaServicoViewModel, CATEGORIA_SERVICO>();
            CreateMap<DepartamentoViewModel, DEPARTAMENTO>();
            CreateMap<ServicoViewModel, SERVICO>();
            CreateMap<AtendimentoAcompanhamentoViewModel, ATENDIMENTO_ACOMPANHAMENTO>();
            CreateMap<AtendimentoViewModel, ATENDIMENTO>();

        }
    }
}