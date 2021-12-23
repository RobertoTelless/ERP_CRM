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
            //CreateMap<ClienteViewModel, CLIENTE>();
            //CreateMap<ClienteContatoViewModel, CLIENTE_CONTATO>();
            //CreateMap<MensagemViewModel, MENSAGENS>();
            CreateMap<GrupoViewModel, GRUPO>();
            CreateMap<GrupoContatoViewModel, GRUPO_CLIENTE>();
            //CreateMap<CategoriaClienteViewModel, CATEGORIA_CLIENTE>();
            //CreateMap<PosicaoViewModel, POSICAO>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            //CreateMap<CRMViewModel, CRM>();
            //CreateMap<CRMContatoViewModel, CRM_CONTATO>();
            //CreateMap<CRMComentarioViewModel, CRM_COMENTARIO>();
            //CreateMap<CRMAcaoViewModel, CRM_ACAO>();
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

        }
    }
}