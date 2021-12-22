using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_CRM_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<NOTIFICACAO, NotificacaoViewModel>();
            //CreateMap<CLIENTE, ClienteViewModel>();
            //CreateMap<CLIENTE_CONTATO, ClienteContatoViewModel>();
            //CreateMap<MENSAGENS, MensagemViewModel>();
            //CreateMap<GRUPO, GrupoViewModel>();
            //CreateMap<GRUPO_CLIENTE, GrupoContatoViewModel>();
            //CreateMap<CATEGORIA_CLIENTE, CategoriaClienteViewModel>();
            //CreateMap<POSICAO, PosicaoViewModel>();
            CreateMap<TEMPLATE, TemplateViewModel>();
            //CreateMap<CRM, CRMViewModel>();
            //CreateMap<CRM_CONTATO, CRMContatoViewModel>();
            //CreateMap<CRM_COMENTARIO, CRMComentarioViewModel>();
            //CreateMap<CRM_ACAO, CRMAcaoViewModel>();
            CreateMap<AGENDA, AgendaViewModel>();
            CreateMap<PLANO, PlanoViewModel>();
            CreateMap<ASSINANTE, AssinanteViewModel>();
            CreateMap<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>();
            CreateMap<FILIAL, FilialViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();
            CreateMap<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>();
            CreateMap<TELEFONE, TelefoneViewModel>();
            CreateMap<TAREFA, TarefaViewModel>();
            CreateMap<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>();
            CreateMap<CLIENTE, ClienteViewModel>();
            CreateMap<CLIENTE_CONTATO, ClienteContatoViewModel>();
            CreateMap<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>();
            CreateMap<CLIENTE_TAG, ClienteTagViewModel>();
            CreateMap<FORNECEDOR, FornecedorViewModel>();
            CreateMap<FORNECEDOR_CONTATO, FornecedorContatoViewModel>();
            CreateMap<FORNECEDOR_COMENTARIO, FornecedorComentarioViewModel>();

        }
    }
}
