using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.WorkClasses;

namespace ApplicationServices.Interfaces
{
    public interface IPedidoCompraAppService : IAppServiceBase<PEDIDO_COMPRA>
    {
        Int32 ValidateCreate(PEDIDO_COMPRA perfil, USUARIO usuario);
        Int32 ValidateEdit(PEDIDO_COMPRA perfil, PEDIDO_COMPRA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PEDIDO_COMPRA item, PEDIDO_COMPRA itemAntes);
        Int32 ValidateDelete(PEDIDO_COMPRA perfil, USUARIO usuario);
        Int32 ValidateReativar(PEDIDO_COMPRA perfil, USUARIO usuario);
        Int32 ValidateCreateAcompanhamento(PEDIDO_COMPRA_ACOMPANHAMENTO item);

        List<PEDIDO_COMPRA> GetAllItens();
        List<PEDIDO_COMPRA> GetAllItensAdm();
        List<PEDIDO_COMPRA> GetAllItensAdmUser(Int32 id);
        PEDIDO_COMPRA GetItemById(Int32 id);
        PEDIDO_COMPRA GetByNome(String nome);
        List<PEDIDO_COMPRA> GetByUser(Int32 id);
        PEDIDO_COMPRA CheckExist(PEDIDO_COMPRA conta);

        List<FORMA_PAGAMENTO> GetAllFormas();
        List<UNIDADE> GetAllUnidades();
        List<FILIAL> GetAllFilial();
        PEDIDO_COMPRA_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? usuaId, String nome, String numero, String nf, DateTime? data, DateTime? dataPrevista, Int32? status, out List<PEDIDO_COMPRA> objeto);
        Int32 ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, out List<PEDIDO_COMPRA> objeto);
        List<PEDIDO_COMPRA> GetEncerrados();
        List<PEDIDO_COMPRA> GetAtrasados();
        List<PEDIDO_COMPRA> GetCancelados();

        ITEM_PEDIDO_COMPRA GetItemCompraById(Int32 id);
        Int32 ValidateEditItemCompra(ITEM_PEDIDO_COMPRA item);
        Int32 ValidateDeleteItemCompra(ITEM_PEDIDO_COMPRA item);
        Int32 ValidateReativarItemCompra(ITEM_PEDIDO_COMPRA item);
        Int32 ValidateCreateItemCompra(ITEM_PEDIDO_COMPRA item);
        Int32 ValidateEnvioCotacao(PEDIDO_COMPRA item, String emailPersonalizado, USUARIO usuario);
        Int32 ValidateEnvioCotacao(PEDIDO_COMPRA item, List<AttachmentForn> anexo, String emailPersonalizado, USUARIO usuario);
        Int32 ValidateCotacao(PEDIDO_COMPRA item, USUARIO usuario);
        String ValidateCreateMensagem(FORNECEDOR item, USUARIO usuario, Int32? idAss);
        Int32 ValidateEditItemCompraCotacao(ITEM_PEDIDO_COMPRA item);
        Int32 ValidateAprovacao(PEDIDO_COMPRA item);
        Int32 ValidateReprovacao(PEDIDO_COMPRA item);
        Int32 ValidateCancelamento(PEDIDO_COMPRA item);
        Int32 ValidateEnvioAprovacao(PEDIDO_COMPRA item);
        Int32 ValidateReceber(PEDIDO_COMPRA item);
        Int32 ValidateRecebido(PEDIDO_COMPRA item);
        Int32 ValidateItemRecebido(ITEM_PEDIDO_COMPRA item);
    }
}
