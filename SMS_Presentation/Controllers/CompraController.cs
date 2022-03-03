using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SMS_Presentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_CRM_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using CrossCutting;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net.Mail;


namespace ERP_CRM_Solution.Controllers
{
    public class CompraController : Controller
    {
        private readonly IPedidoCompraAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IFornecedorAppService forApp;
        private readonly IProdutoAppService proApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly ITemplateAppService tempApp;
        private readonly IProdutotabelaPrecoAppService ptpApp;
        private String msg;
        private Exception exception;

        PEDIDO_COMPRA objeto = new PEDIDO_COMPRA();
        PEDIDO_COMPRA objetoAntes = new PEDIDO_COMPRA();
        List<PEDIDO_COMPRA> listaMaster = new List<PEDIDO_COMPRA>();
        String extensao;

        public CompraController(IPedidoCompraAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, ICentroCustoAppService ccApps, IFornecedorAppService forApps, IProdutoAppService proApps, IContaBancariaAppService cbApps, ITemplateAppService tempApps, IProdutotabelaPrecoAppService ptpApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            ccApp = ccApps;
            forApp = forApps;
            proApp = proApps;
            cbApp = cbApps;
            tempApp = tempApps;
            ptpApp = ptpApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public JsonResult ValorTotal(Int32 qtde, Decimal valor)
        {
            return Json(qtde * valor);
        }

        [HttpPost]
        public JsonResult GetCustoProduto(Int32 id, Int32? fili)
        {
            var result = new Hashtable();
            if (fili != null)
            {
                var prod = proApp.GetById(id).PRODUTO_TABELA_PRECO.First(x => x.FILI_CD_ID == fili);
                result.Add("custo", prod.PRTP_VL_CUSTO == null ? 0 : prod.PRTP_VL_CUSTO);
                result.Add("markup", prod.PRTP_NR_MARKUP == null ? 0 : prod.PRTP_NR_MARKUP);
                result.Add("unidade", prod.PRODUTO.UNIDADE.UNID_NM_NOME);
            }
            else
            {
                var prod = proApp.GetById(id);
                result.Add("custo", prod.PROD_VL_CUSTO == null ? 0 : prod.PROD_VL_CUSTO);
                result.Add("markup", prod.PROD_VL_MARKUP_PADRAO == null ? 0 : prod.PROD_VL_MARKUP_PADRAO);
                result.Add("unidade", prod.UNIDADE == null ? "" : prod.UNIDADE.UNID_NM_NOME);
            }
            return Json(result);
        }

        public FileResult DownloadPedidoCompra(Int32 id)
        {
            PEDIDO_COMPRA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.PECA_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpGet]
        public ActionResult MontarTelaPedidoCompra()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["IdCompra"] != null)
            {
                Session["ListaCompra"] = baseApp.GetAllItens(idAss).Where(x => x.PECO_CD_ID == (Int32)Session["IdCompra"]).ToList();
                Session["IdCompra"] = 0;
            }
            if (Session["ListaCompra"] == null || ((List<PEDIDO_COMPRA>)Session["ListaCompra"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCompra"] = listaMaster;
            }

            ViewBag.Listas = (List<PEDIDO_COMPRA>)Session["ListaCompra"];
            ViewBag.Title = "Pedidos de Compra";

            // Indicadores
            List<PEDIDO_COMPRA> lista = baseApp.GetAllItens(idAss);
            ViewBag.Lista = lista;
            ViewBag.Pedidos = lista.Count;
            ViewBag.Encerradas = lista.Count(p => p.PECO_IN_STATUS == 7);
            ViewBag.Canceladas = lista.Count(p => p.PECO_IN_STATUS == 8);
            ViewBag.Atrasadas = lista.Count(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8);
            ViewBag.EncerradasLista = lista.Where(p => p.PECO_IN_STATUS == 7).ToList();
            ViewBag.CanceladasLista = lista.Where(p => p.PECO_IN_STATUS == 8).ToList();
            ViewBag.AtrasadasLista = lista.Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Aprovador = usuario.USUA_IN_APROVADOR;
            ViewBag.Comprador = usuario.USUA_IN_COMPRADOR;
            ViewBag.CotacaoEnvioSucesso = (Int32)Session["SMSEmailEnvio"];

            Session["Fornecedores"] = forApp.GetAllItens(idAss);
            Session["Produtos"] = proApp.GetAllItens(idAss);
            Session["SMSEmailEnvio"] = 0;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                if ((Int32)Session["MensCompra"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCompra"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0099", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCompra"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0100", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCompra"] == 50)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0125", CultureInfo.CurrentCulture));
                }

            }

            // Carrega listas
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Para Receber", Value = "5" });
            status.Add(new SelectListItem() { Text = "Em Recebimento", Value = "6" });
            status.Add(new SelectListItem() { Text = "Recebido", Value = "7" });
            status.Add(new SelectListItem() { Text = "Cancelado", Value = "8" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            // Abre view
            Session["MensCompra"] = 0;
            objeto = Session["FiltroCompra"] == null ? new PEDIDO_COMPRA() : (PEDIDO_COMPRA)Session["FiltroCompra"];
            objeto.PECO_DT_DATA = null;
            return View(objeto);
        }

        public ActionResult RetirarFiltroPedidoCompra()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroCompra"] = null;
            Session["ListaCompra"] = null;
            return RedirectToAction("MontarTelaPedidoCompra");
        }

        public ActionResult MostrarTudoPedidoCompra()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaCompra"] = listaMaster;
            return RedirectToAction("MontarTelaPedidoCompra");
        }

        [HttpPost]
        public ActionResult FiltrarPedidoCompra(PEDIDO_COMPRA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                Session["FiltroCompra"] = item;
                Session["IdCompra"] = null;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilter(item.USUA_CD_ID, item.PECO_NM_NOME, item.PECO_NR_NUMERO, item.PECO_NR_NOTA_FISCAL, item.PECO_DT_DATA, item.PECO_DT_PREVISTA, item.PECO_IN_STATUS, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaPedidoCompra");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaCompra"] = listaObj;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaPedidoCompra");

            }
        }

        public ActionResult VoltarBaseMontarTelaPedidoCompra()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPedidoCompra");
        }

        [HttpPost]
        public void MontaListaItemPedido(ITEM_PEDIDO_COMPRA item)
        {
            if (Session["ListaITPC"] == null)
            {
                Session["ListaITPC"] = new List<ITEM_PEDIDO_COMPRA>();
            }
            List<ITEM_PEDIDO_COMPRA> lit = (List<ITEM_PEDIDO_COMPRA>)Session["ListaITPC"];
            lit.Add(item);
            Session["ListaITPC"] = lit;

            //Task.Run(() =>
            //{            
            //    if (Session["ListaITPC"] == null)
            //    {
            //        Session["ListaITPC"] = new List<ITEM_PEDIDO_COMPRA>();
            //    }
            //    List<ITEM_PEDIDO_COMPRA> lit = (List<ITEM_PEDIDO_COMPRA>)Session["ListaITPC"];
            //    lit.Add(item);
            //    Session["ListaITPC"] = lit;
            //});
        }

        [HttpPost]
        public void RemoveItpcTabela(ITEM_PEDIDO_COMPRA item)
        {
            if (Session["ListaITPC"] != null)
            {
                List<ITEM_PEDIDO_COMPRA> lit = (List<ITEM_PEDIDO_COMPRA>)Session["ListaITPC"];
                if (item.ITPC_IN_TIPO == 1)
                {
                    lit.RemoveAll(x => x.PROD_CD_ID == item.PROD_CD_ID);
                }
                Session["ListaITPC"] = lit;
            }
        }

        [HttpGet]
        public ActionResult IncluirPedidoCompra()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCompra"] = 2;
                    return RedirectToAction("MontarTelaPedidoCompra", "Compra");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaITPC"] = null;

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumCompra"] <= num)
            {
                Session["MensCompra"] = 50;
                return RedirectToAction("MontarTelaCompra", "Compra");
            }

            // Prepara listas
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(p => p.CECU_NM_NOME), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllFormas(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");

            // Prepara view
            Session["VoltaPop"] = 1;
            Session["ListaITPC"] = null;
            PEDIDO_COMPRA item = new PEDIDO_COMPRA();
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.PECO_DT_DATA = DateTime.Today.Date;
            vm.PECO_IN_ATIVO = 1;
            vm.PECO_IN_STATUS = 1;
            vm.PECO_IN_TIPO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.PECO_DT_DATA = DateTime.Today.Date;
            vm.PECO_DT_PREVISTA = DateTime.Today.Date.AddDays(30);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(p => p.CECU_NM_NOME), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllFormas(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            Hashtable result = new Hashtable();

            if (ModelState.IsValid)
            {
                if (Session["ListaITPC"] == null)
                {
                    ModelState.AddModelError("", "Nenhum Item de Pedido cadastrado no pedido");
                    return View(vm);
                }

                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCompra"] = 3;
                        return RedirectToAction("MontarTelaPedidoCompra");
                    }

                    // Acerta numero do pedido
                    item.PECO_NR_NUMERO = item.PECO_CD_ID.ToString();
                    volta = baseApp.ValidateEdit(item, item, usuario);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    
                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    Session["ListaCompra"] = null;

                    foreach (var itpc in (List<ITEM_PEDIDO_COMPRA>)Session["ListaITPC"])
                    {
                        itpc.ITPC_NR_QUANTIDADE_REVISADA = itpc.ITPC_QN_QUANTIDADE;
                        itpc.ITPC_IN_ATIVO = 1;
                        itpc.PECO_CD_ID = item.PECO_CD_ID;

                        if (itpc.ITPC_IN_TIPO == 1)
                        {
                            PRODUTO prod = proApp.GetItemById((Int32)itpc.PROD_CD_ID);
                            itpc.UNID_CD_ID = prod.UNID_CD_ID;
                            itpc.ITPC_VL_PRECO_SELECIONADO = prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).Count() > 0 ? prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).First().PRTP_VL_CUSTO : null;
                        }
                        Int32 voltaItem = baseApp.ValidateCreateItemCompra(itpc);
                    }

                    //Session["ListaITPC"] = null;
                    Session["IdVolta"] = item.PECO_CD_ID;
                    if (Session["FileQueueCompra"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCompra"];

                        foreach(var file in fq)
                        {
                            UploadFileQueuePedidoCompra(file);
                        }

                        Session["FileQueueCompra"] = null;
                    }

                    Session["IdCompra"] = item.PECO_CD_ID;
                    //return RedirectToAction("EditarPedidoCompra", new { id = (Int32)Session["IdCompra"] });
                    return RedirectToAction("MontarTelaPedidoCompra");

                }
                catch (Exception ex)
                {
                    Session["ListaITPC"] = null;
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                Session["ListaITPC"] = null;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirCompraExpressa()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCompra"] = 2;
                    return RedirectToAction("MontarTelaPedidoCompra", "Compra");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaITPC"] = null;

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumCompra"] <= num)
            {
                Session["MensCompra"] = 50;
                return RedirectToAction("MontarTelaCompra", "Compra");
            }

            // Prepara listas
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(p => p.CECU_NM_NOME), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllFormas(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");

            // Prepara view
            Session["VoltaPop"] = 1;
            PEDIDO_COMPRA item = new PEDIDO_COMPRA();
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.PECO_DT_DATA = DateTime.Today.Date;
            vm.PECO_IN_ATIVO = 1;
            vm.PECO_IN_STATUS = 5;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.PECO_DT_DATA = DateTime.Today.Date;
            vm.PECO_DT_PREVISTA = DateTime.Today.Date.AddDays(30);
            vm.PECO_IN_TIPO = 2;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IncluirCompraExpressa(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(p => p.CECU_NM_NOME), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllFormas(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            Hashtable result = new Hashtable();

            if (ModelState.IsValid)
            {
                if (Session["ListaITPC"] == null)
                {
                    ModelState.AddModelError("", "Nenhum Item de Pedido cadastrado no pedido");
                    return View(vm);
                }

                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCompra"] = 3;
                        return RedirectToAction("MontarTelaPedidoCompra");
                    }

                    // Acerta numero do pedido
                    item.PECO_NR_NUMERO = item.PECO_CD_ID.ToString();
                    volta = baseApp.ValidateEdit(item, item, usuario);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    
                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    Session["ListaCompra"] = null;

                    foreach (var itpc in (List<ITEM_PEDIDO_COMPRA>)Session["ListaITPC"])
                    {
                        itpc.ITPC_NR_QUANTIDADE_REVISADA = itpc.ITPC_QN_QUANTIDADE;
                        itpc.ITPC_IN_ATIVO = 1;
                        itpc.PECO_CD_ID = item.PECO_CD_ID;

                        if (itpc.ITPC_IN_TIPO == 1)
                        {
                            PRODUTO prod = proApp.GetItemById((Int32)itpc.PROD_CD_ID);
                            itpc.UNID_CD_ID = prod.UNID_CD_ID;
                            itpc.ITPC_VL_PRECO_SELECIONADO = prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).Count() > 0 ? prod.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == item.FILI_CD_ID).First().PRTP_VL_CUSTO : null;
                        }
                        Int32 voltaItem = baseApp.ValidateCreateItemCompra(itpc);
                    }

                    Session["ListaITPC"] = null;
                    Session["IdVolta"] = item.PECO_CD_ID;
                    if (Session["FileQueueCompra"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCompra"];

                        foreach(var file in fq)
                        {
                            UploadFileQueuePedidoCompra(file);
                        }

                        Session["FileQueueCompra"] = null;
                    }
                    Session["IdCompra"] = item.PECO_CD_ID;

                    // Acerta estoque
                    PEDIDO_COMPRA ped = baseApp.GetItemById(item.PECO_CD_ID);
                    Int32 volta1 = baseApp.ValidateRecebido(ped, usuario);


                    // Gera conta pagar
                    CONTA_PAGAR cp = new CONTA_PAGAR();

                    // Recupera fornecedor
                    FORNECEDOR forn = forApp.GetItemById((Int32)ped.FORN_CD_ID);

                    // Calcula valor
                    Decimal valor = 0;

                    foreach (var i in ped.ITEM_PEDIDO_COMPRA)
                    {
                        if (i.ITPC_VL_PRECO_SELECIONADO != null)
                        {
                            valor += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (decimal)i.ITPC_VL_PRECO_SELECIONADO;
                        }
                    }

                    // Gera CP
                    cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    cp.CAPA_DS_DESCRICAO = "Lançamento a pagar referente ao pedido de compra " + ped.PECO_NM_NOME + " de número " + ped.PECO_NR_NUMERO;
                    cp.CAPA_DT_COMPETENCIA = DateTime.Today.Date;
                    cp.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
                    cp.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
                    cp.CAPA_IN_ATIVO = 1;
                    cp.CAPA_IN_LIQUIDADA = 0;
                    cp.CAPA_IN_PAGA_PARCIAL = 0;
                    cp.CAPA_IN_PARCELADA = 0;
                    cp.CAPA_IN_PARCELAS = 0;
                    cp.CAPA_IN_TIPO_LANCAMENTO = 1;
                    cp.CAPA_NR_DOCUMENTO = ped.PECO_NR_NUMERO;
                    cp.CAPA_VL_DESCONTO = 0;
                    cp.CAPA_VL_JUROS = 0;
                    cp.CAPA_VL_PARCELADO = 0;
                    cp.CAPA_VL_PARCIAL = 0;
                    cp.CAPA_VL_TAXAS = 0;
                    cp.CAPA_VL_VALOR_PAGO = 0;
                    cp.CAPA_VL_VALOR = valor;
                    cp.CAPA_VL_SALDO = valor;
                    cp.CECU_CD_ID = ped.CECU_CD_ID;
                    cp.FOPA_CD_ID = 1;
                    cp.FORN_CD_ID = forn.FORN_CD_ID;
                    cp.PECO_CD_ID = ped.PECO_CD_ID;
                    cp.USUA_CD_ID = ped.USUA_CD_ID;
                    cp.COBA_CD_ID = cbApp.GetContaPadrao(idAss).COBA_CD_ID;

                    listaMaster = new List<PEDIDO_COMPRA>();
                    Session["ListaCompra"] = null;
                    Session["ContaPagar"] = cp;
                    Session["VoltaCompra"] = 1;
                    Session["IdCompra"] = item.PECO_CD_ID;
                    return RedirectToAction("IncluirCP", "ContaPagar", new { voltaCompra = 1 });
                }
                catch (Exception ex)
                {
                    Session["ListaITPC"] = null;
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                Session["ListaITPC"] = null;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarPedidoCompra(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCompra"] = 2;
                    return RedirectToAction("MontarTelaPedidoCompra", "Compra");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(p => p.CECU_NM_NOME), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllFormas(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 12)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 11)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0101", CultureInfo.CurrentCulture));
            }
            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 5)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", "Último item não pode ser desativado");
            }
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 2;

            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            objetoAntes = item;
            Session["PedidoCompra"] = item;
            Session["IdVolta"] = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.VALOR_TOTAL = custo;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(p => p.CECU_NM_NOME), "CECU_CD_ID", "CECU_NM_NOME");
            ViewBag.Filiais = new SelectList(baseApp.GetAllFilial(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Formas = new SelectList(baseApp.GetAllFormas(idAss), "FOPA_CD_ID", "FOPA_NM_NOME");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Para Cotação", Value = "1" });
            status.Add(new SelectListItem() { Text = "Em Cotação", Value = "2" });
            status.Add(new SelectListItem() { Text = "Para Aprovação", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aprovada", Value = "4" });
            status.Add(new SelectListItem() { Text = "Encerrada", Value = "5" });
            status.Add(new SelectListItem() { Text = "Cancelada", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<PRODUTO> lista = proApp.GetAllItens(idAss).OrderBy(x => x.PROD_NM_NOME).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Produtos = new SelectList(lista, "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    Session["ListaCompra"] = null;
                    Session["IdCompra"] = item.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPedidoCompra(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCompra"] = 2;
                    return RedirectToAction("MontarTelaPedidoCompra", "Compra");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            Session["MensCompra"] = 0;
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuario);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 4;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }

                // Sucesso
                Session["MensCompra"] = 0;
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarPedidoCompra(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCompra"] = 2;
                    return RedirectToAction("MontarTelaPedidoCompra", "Compra");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumCompra"] <= num)
            {
                Session["MensCompra"] = 50;
                return RedirectToAction("MontarTelaCompra", "Compra");
            }

            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarPedidoCompra(PedidoCompraViewModel vm)
        {

            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            PEDIDO_COMPRA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult IncluirProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            Session["VoltaProduto"] = 4;
            return RedirectToAction("IncluirProduto", "Produto");
        }

        public ActionResult VoltarAnexoPedidoCompra()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            if ((Int32)Session["VoltaCompra"] == 1)
            {
                return RedirectToAction("VerPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 3)
            {
                return RedirectToAction("CancelarPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 4)
            {
                return RedirectToAction("EnviarCotacaoPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 5)
            {
                return RedirectToAction("ReceberPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 6)
            {
                return RedirectToAction("AprovarPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 7)
            {
                return RedirectToAction("ProcessarReceberPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 8)
            {
                return RedirectToAction("ProcessarEnviarAprovacaoPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            if ((Int32)Session["VoltaCompra"] == 9)
            {
                return RedirectToAction("ProcessarCotacaoPedidoCompra", new { id = (Int32)Session["IdVolta"] });
            }
            return RedirectToAction("EditarPedidoCompra", new { id = (Int32)Session["IdVolta"] });
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                queue.Add(f);
            }

            Session["FileQueueCompra"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueuePedidoCompra(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCompra"] = 12;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }

            PEDIDO_COMPRA item = baseApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensCompra"] = 11;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = file.ContentType;
            String a = extensao;

            // Gravar registro
            PEDIDO_COMPRA_ANEXO foto = new PEDIDO_COMPRA_ANEXO();
            foto.PECA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PECA_DT_ANEXO = DateTime.Today;
            foto.PECA_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.PECA_IN_TIPO = tipo;
            foto.PECA_NM_TITULO = fileName;
            foto.PECO_CD_ID = item.PECO_CD_ID;

            item.PEDIDO_COMPRA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoPedidoCompra");
        }

        [HttpPost]
        public ActionResult UploadFilePedidoCompra(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCompra"] = 12;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }

            PEDIDO_COMPRA item = baseApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensCompra"] = 11;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/PedidoCompra/" + item.PECO_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PEDIDO_COMPRA_ANEXO foto = new PEDIDO_COMPRA_ANEXO();
            foto.PECA_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PECA_DT_ANEXO = DateTime.Today;
            foto.PECA_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.PECA_IN_TIPO = tipo;
            foto.PECA_NM_TITULO = fileName;
            foto.PECO_CD_ID = item.PECO_CD_ID;

            item.PEDIDO_COMPRA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoPedidoCompra");
        }

        public void ProcessarFornecedor(Int32 id)
        {
            Session["FornCotacao"] = id;
        }

        [HttpGet]
        public ActionResult ProcessarCotacaoPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usu = (USUARIO)Session["UserCredentials"];

            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            List<FORNECEDOR> forn = new List<FORNECEDOR>();
            foreach (var itpc in item.ITEM_PEDIDO_COMPRA)
            {
                if (itpc.PRODUTO != null)
                {
                    forn.AddRange(itpc.PRODUTO.PRODUTO_FORNECEDOR.Where(x => x.PRFO_IN_ATIVO == 1).Select(x => x.FORNECEDOR));
                }

            }
            Session["CotForn"] = forn.Distinct().ToList<FORNECEDOR>();
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["CotForn"];
            objeto = item;
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            Session["CustoTotal"] = custo;
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["VmAntes"] = vm;
            Session["IdVolta"] = id;
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 9;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ProcessarCotacaoPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["CotForn"];
            List<FORNECEDOR> lf = (List<FORNECEDOR>)Session["CotForn"];
            ViewBag.CustoTotal = (Decimal)Session["CustoTotal"];
            try
            {
                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);

                if (baseApp.GetById(vm.PECO_CD_ID).ITEM_PEDIDO_COMPRA.Any(x => x.ITPC_VL_PRECO_SELECIONADO == null))
                {
                    ModelState.AddModelError("", "Há itens sem preço selecionado no pedido");
                    return View((PedidoCompraViewModel)Session["VmAntes"]);
                }
                if (baseApp.GetById(vm.PECO_CD_ID).ITEM_PEDIDO_COMPRA.Count == 0)
                {
                    ModelState.AddModelError("", "Pedido de compra não possui itens");
                    return View((PedidoCompraViewModel)Session["VmAntes"]);
                }

                //item.FORN_CD_ID = (Int32)Session["FornCotacao"];
                item.FORN_CD_ID = lf.First().FORN_CD_ID;

                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateCotacao(item, usuario);

                if (volta == 0)
                {
                    foreach (var itpc in item.ITEM_PEDIDO_COMPRA)
                    {
                        try
                        {
                            ITEM_PEDIDO_COMPRA itpcCotado = new ITEM_PEDIDO_COMPRA();
                            itpcCotado.ITPC_CD_ID = itpc.ITPC_CD_ID;
                            itpcCotado.PECO_CD_ID = itpc.PECO_CD_ID;
                            itpcCotado.PROD_CD_ID = itpc.PROD_CD_ID;
                            itpcCotado.UNID_CD_ID = itpc.UNID_CD_ID;
                            itpcCotado.ITPC_QN_QUANTIDADE = itpc.ITPC_QN_QUANTIDADE;
                            itpcCotado.ITPC_VL_PRECO_SELECIONADO = itpc.ITPC_VL_PRECO_SELECIONADO;
                            itpcCotado.ITPC_NR_QUANTIDADE_REVISADA = itpc.ITPC_NR_QUANTIDADE_REVISADA;
                            itpcCotado.ITPC_TX_OBSERVACOES = itpc.ITPC_TX_OBSERVACOES;
                            itpcCotado.ITPC_IN_ATIVO = itpc.ITPC_IN_ATIVO;
                            itpcCotado.ITPC_DT_COTACAO = DateTime.Now;
                            itpcCotado.ITPC_IN_TIPO = 1;
                            itpcCotado.ITPC_DS_JUSTIFICATIVA = itpc.ITPC_DS_JUSTIFICATIVA;
                            itpcCotado.ITPC_NR_QUANTIDADE_RECEBIDA = itpc.ITPC_NR_QUANTIDADE_RECEBIDA;

                            Int32 voltaITPC = baseApp.ValidateEditItemCompra(itpcCotado);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            return View(objeto);
                        }
                    }
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                Session["IdCompra"] = item.PECO_CD_ID;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult AprovarPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            Session["IdVolta"] = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult AprovarPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateAprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0103", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0104", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0105", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                Session["IdCompra"] = item.PECO_CD_ID;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReprovarPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            Session["IdVolta"] = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReprovarPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateReprovacao(item);

                // Verifica retorno
                if (volta == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0103", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0104", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0105", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ProcessarReceberPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["VoltaCompra"] = 7;
            Session["IdVolta"] = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ProcessarReceberPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateReceber(item);
                Int32 volta1 = baseApp.ValidateRecebido(item, usuario);

                // Verifica retorno
                CONTA_PAGAR cp = new CONTA_PAGAR();
                if (volta == 0)
                {
                    // Recupera fornecedor
                    FORNECEDOR forn = forApp.GetItemById((Int32)item.FORN_CD_ID);

                    // Calcula valor
                    Decimal valor = 0;

                    foreach (var i in item.ITEM_PEDIDO_COMPRA)
                    {
                        if (i.ITPC_VL_PRECO_SELECIONADO != null)
                        {
                            valor += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (decimal)i.ITPC_VL_PRECO_SELECIONADO;
                        }
                    }

                    // Gera CP
                    cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    cp.CAPA_DS_DESCRICAO = "Lançamento a pagar referente ao pedido de compra " + item.PECO_NM_NOME + " de número " + item.PECO_NR_NUMERO;
                    cp.CAPA_DT_COMPETENCIA = DateTime.Today.Date;
                    cp.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
                    cp.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
                    cp.CAPA_IN_ATIVO = 1;
                    cp.CAPA_IN_LIQUIDADA = 0;
                    cp.CAPA_IN_PAGA_PARCIAL = 0;
                    cp.CAPA_IN_PARCELADA = 0;
                    cp.CAPA_IN_PARCELAS = 0;
                    cp.CAPA_IN_TIPO_LANCAMENTO = 1;
                    cp.CAPA_NR_DOCUMENTO = item.PECO_NR_NOTA_FISCAL;
                    cp.CAPA_VL_DESCONTO = 0;
                    cp.CAPA_VL_JUROS = 0;
                    cp.CAPA_VL_PARCELADO = 0;
                    cp.CAPA_VL_PARCIAL = 0;
                    cp.CAPA_VL_TAXAS = 0;
                    cp.CAPA_VL_VALOR_PAGO = 0;
                    cp.CAPA_VL_VALOR = valor;
                    cp.CAPA_VL_SALDO = valor;
                    cp.CECU_CD_ID = item.CECU_CD_ID;
                    cp.FOPA_CD_ID = 1;
                    cp.FORN_CD_ID = forn.FORN_CD_ID;
                    cp.PECO_CD_ID = item.PECO_CD_ID;
                    cp.USUA_CD_ID = item.USUA_CD_ID;
                    cp.COBA_CD_ID = cbApp.GetContaPadrao(idAss).COBA_CD_ID;
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                Session["ContaPagar"] = cp;
                Session["VoltaCompra"] = 1;
                Session["IdCompra"] = item.PECO_CD_ID;
                return RedirectToAction("IncluirCP", "ContaPagar", new { voltaCompra = 1 });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReceberPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["VoltaCompra"] = 5;
            Session["IdVolta"] = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReceberPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                    Decimal custo = 0;
                    foreach (var i in item.ITEM_PEDIDO_COMPRA)
                    {
                        if (i.ITPC_VL_PRECO_SELECIONADO != null)
                        {
                            custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                        }
                    }
                    ViewBag.CustoTotal = custo;
                    Int32 volta = baseApp.ValidateRecebido(item, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<PEDIDO_COMPRA>();
                    Session["ListaCompra"] = null;
                    Session["IdCompra"] = item.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReceberItemPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReceberItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                if (vm.ITPC_NR_QUANTIDADE_REVISADA != vm.ITPC_NR_QUANTIDADE_RECEBIDA && vm.ITPC_DS_JUSTIFICATIVA == null)
                {
                    ModelState.AddModelError("", "Para quantidade recebida diferente do previsto, necessário justificativa");
                    return View(vm);
                }

                // Executa a operação
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateItemRecebido(item, usuario);

                // Verifica retorno
                if (volta == 2)
                {
                    Session["PedidoRecebido"] = vm.PECO_CD_ID;
                    return RedirectToAction("MontarTelaPedidoCompra");
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                return RedirectToAction("ReceberPedidoCompra", new { id = Session["IdVolta"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult CancelarPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];


            // Prepara view
            ViewBag.Fornecedores = new SelectList(((List<FORNECEDOR>)Session["Fornecedores"]).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            Session["CustoTotal"] = custo;
            if ((Int32)Session["MensCompra"] == 20)
            {
                Session["MensCompra"] = 0;
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0106", CultureInfo.CurrentCulture));
            }
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 3;
            Session["IdVolta"] = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            vm.VALOR_TOTAL = custo;
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelarPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.Fornecedores = new SelectList(((List<FORNECEDOR>)Session["Fornecedores"]).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            vm.VALOR_TOTAL = (decimal)Session["CustoTotal"];
            Session["CustoTotal"] = 0;
            try
            {
                if (vm.FORN_CD_ID == 0)
                {
                    ModelState.AddModelError("", "Fornecedor não selecionado");
                    return View(vm);
                }

                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateCancelamento(item);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 20;
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0106", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult VerPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["IdVolta"] = id;
            Session["VoltaCompra"] = 1;
            return View(vm);
        }

        public ActionResult VerItemPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        public ActionResult VerAtrasados()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Usuarios = new SelectList(((List<USUARIO>)Session["Usuarios"]).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            if (Session["ListaCompraAtrasados"] == null || ((List<PEDIDO_COMPRA>)Session["ListaCompraAtrasados"]).Count == 0)
            {
                Session["ListaCompraAtrasados"] = baseApp.GetAllItens(idAss).Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList();
            }

            // Abre view
            List<PEDIDO_COMPRA> lista = (List<PEDIDO_COMPRA>)Session["ListaCompraAtrasados"];
            ViewBag.Pedidos = ((List<PEDIDO_COMPRA>)Session["ListaCompraAtrasados"]).Count;
            ViewBag.Atrasadas = lista.Count(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8);
            ViewBag.AtrasadasLista = lista.Where(p => p.PECO_DT_PREVISTA < DateTime.Today.Date && p.PECO_IN_STATUS != 7 && p.PECO_IN_STATUS != 8).ToList();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensCompra"] = null;
            }

            objeto = new PEDIDO_COMPRA();
            Session["VoltaCompra"] = 1;
            Session["VoltaConsulta"] = 3;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarAtrasados(PEDIDO_COMPRA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                Session["FiltroCompraAtrasados"] = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilterDash(item.PECO_NR_NUMERO, item.PECO_DT_FINAL, item.PECO_NM_NOME, item.USUA_CD_ID, null, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("VerAtrasados");
                }

                // Sucesso
                Session["ListaCompraAtrasados"]= listaObj;
                return RedirectToAction("VerAtrasados");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerAtrasados");

            }
        }

        public ActionResult RetirarFiltroAtrasados()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            Session["FiltroCompraAtrasados"] = null;
            Session["ListaCompraAtrasados"] = null;
            return RedirectToAction("VerAtrasados");
        }

        public ActionResult VerEncerrados()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Usuarios = new SelectList(((List<USUARIO>)Session["Usuarios"]).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            if (Session["ListaCompraEncerrados"] == null || ((List<PEDIDO_COMPRA>)Session["ListaCompraEncerrados"]).Count == 0)
            {
                Session["ListaCompraEncerrados"] = baseApp.GetAllItens(idAss).Where(p => p.PECO_IN_STATUS == 7).ToList();
            }

            // Abre view
            List<PEDIDO_COMPRA> lista = (List<PEDIDO_COMPRA>)Session["ListaCompraEncerrados"];

            ViewBag.Pedidos = lista.Count;
            ViewBag.EncerradasLista = lista;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensCompra"] = null;
            }

            objeto = new PEDIDO_COMPRA();
            Session["VoltaCompra"] = 1;
            Session["VoltaConsulta"] = 3;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarEncerrados(PEDIDO_COMPRA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                Session["FiltroCompraEncerrados"] = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilterDash(item.PECO_NR_NUMERO, item.PECO_DT_FINAL, item.PECO_NM_NOME, item.USUA_CD_ID, 7, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("VerEncerrados");
                }

                // Sucesso
                Session["ListaCompraEncerrados"] = listaObj;
                return RedirectToAction("VerEncerrados");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerEncerrados");

            }
        }

        public ActionResult RetirarFiltroEncerrados()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            Session["FiltroCompraEncerrados"] = null;
            Session["ListaCompraEncerrados"] = null;
            return RedirectToAction("VerEncerrados");
        }

        public ActionResult VerCancelados()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Usuarios = new SelectList(((List<USUARIO>)Session["Usuarios"]).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

            if (Session["ListaCompraCancelados"] == null || ((List<PEDIDO_COMPRA>)Session["ListaCompraCancelados"]).Count == 0)
            {
                Session["ListaCompraCancelados"] = baseApp.GetAllItens(idAss).Where(p => p.PECO_IN_STATUS == 8).ToList();
            }

            // Abre view
            List<PEDIDO_COMPRA> lista = (List<PEDIDO_COMPRA>)Session["ListaCompraCancelados"];
            ViewBag.Pedidos = lista.Count;
            ViewBag.CanceladasLista = lista;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCompra"] != null && (Int32)Session["MensCompra"] == 1)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                Session["MensCompra"] = null;
            }

            objeto = new PEDIDO_COMPRA();
            Session["VoltaCompra"] = 1;
            Session["VoltaConsulta"] = 3;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarCancelados(PEDIDO_COMPRA item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                Session["FiltroCompraCancelados"] = item;
                // Executa a operação
                List<PEDIDO_COMPRA> listaObj = new List<PEDIDO_COMPRA>();
                Int32 volta = baseApp.ExecuteFilterDash(item.PECO_NR_NUMERO, item.PECO_DT_FINAL, item.PECO_NM_NOME, item.USUA_CD_ID, 8, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    return RedirectToAction("VerCancelados");
                }

                // Sucesso
                Session["ListaCompraCancelados"] = listaObj;
                return RedirectToAction("VerCancelados");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerCancelados");

            }
        }

        public ActionResult RetirarFiltroCancelados()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            Session["FiltroCompraCancelados"] = null;
            Session["ListaCompraCancelados"] = null;
            return RedirectToAction("VerCancelados");
        }

        [HttpPost]
        public JsonResult AtualizaItemInline(ITEM_PEDIDO_COMPRA item)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Hashtable result = new Hashtable();

            try
            {
                // Executa a operação
                if (item.ITPC_DT_COTACAO == null)
                {
                    item.ITPC_DT_COTACAO = DateTime.Now;
                }
                Int32 volta = baseApp.ValidateEditItemCompra(item);

                // Verifica retorno
                if (volta == 0)
                {
                    result.Add("success", "Item editado com sucesso!");
                    if (item.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        result.Add("vlrTotal", item.ITPC_NR_QUANTIDADE_REVISADA * item.ITPC_VL_PRECO_SELECIONADO);
                    }
                }
                else
                {
                    result.Add("error", "Falha ao editar item");
                }

                Decimal custo = 0;
                foreach (var i in baseApp.GetItemById(item.PECO_CD_ID).ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                result.Add("custoGeral", custo);

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Add("error", ex.Message);
                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult EditarItemPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");

            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            objetoAntes = (PEDIDO_COMPRA)Session["PedidoCompra"];
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateEditItemCompra(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoCompra");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public void EditarItemPedidoCompraInline(Int32? id, Int32? qtde, decimal? preco)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            if (id != null)
            {
                ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById((Int32)id);
                item.ITPC_NR_QUANTIDADE_REVISADA = qtde;
                item.ITPC_VL_PRECO_SELECIONADO = preco;
                Int32 volta = baseApp.ValidateEditItemCompra(item);
            }
        }

        [HttpGet]
        public ActionResult ExcluirItemPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                Int32 volta = baseApp.ValidateDeleteItemCompra(item);

                Session["IdVolta"] = item.PECO_CD_ID;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarItemPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ITEM_PEDIDO_COMPRA item = baseApp.GetItemCompraById(id);
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                PEDIDO_COMPRA ped = baseApp.GetItemById(item.PECO_CD_ID);
                if (item.ITPC_IN_TIPO == 1)
                {

                    if (ped.ITEM_PEDIDO_COMPRA.Where(x => x.PROD_CD_ID == item.PROD_CD_ID && x.ITPC_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        ModelState.AddModelError("", "PRODUTO já existente no pedido");
                        return View(vm);
                    }
                }

                Int32 volta = baseApp.ValidateReativarItemCompra(item);
                Session["IdVolta"] = item.PECO_CD_ID;
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirItemPedidoCompra()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            List<PRODUTO> lista = proApp.GetAllItens(idAss).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");

            ITEM_PEDIDO_COMPRA item = new ITEM_PEDIDO_COMPRA();
            ItemPedidoCompraViewModel vm = Mapper.Map<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>(item);
            vm.PECO_CD_ID = (Int32)Session["IdVolta"];
            vm.ITPC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirItemPedidoCompra(ItemPedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<PRODUTO> lista = proApp.GetAllItens(idAss).Where(p => p.PROD_IN_COMPOSTO == 0).ToList();
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.Produtos = new SelectList(proApp.GetAllItens(idAss).OrderBy(p => p.PROD_NM_NOME), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Unidades = new SelectList(baseApp.GetAllUnidades(idAss).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    vm.ITPC_NR_QUANTIDADE_REVISADA = vm.ITPC_QN_QUANTIDADE;
                    if (vm.ITPC_IN_TIPO == 1)
                    {
                        Int32 a = baseApp.GetItemById(vm.PECO_CD_ID).FILI_CD_ID == null ? (Int32)Session["IdFilial"] : (Int32)baseApp.GetItemById(vm.PECO_CD_ID).FILI_CD_ID;
                        PRODUTO_TABELA_PRECO b = ptpApp.GetByProdFilial((Int32)vm.PROD_CD_ID, a);
                        vm.ITPC_VL_PRECO_SELECIONADO = b == null || b.PRTP_VL_PRECO == null ? 0 : b.PRTP_VL_PRECO;
                    }

                    if (vm.ITPC_IN_TIPO == 1)
                    {
                        var prod = proApp.GetItemById((Int32)vm.PROD_CD_ID);
                        vm.UNID_CD_ID = prod.UNID_CD_ID;
                    }
                    // Executa a operação
                    ITEM_PEDIDO_COMPRA item = Mapper.Map<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>(vm);
                    Int32 volta = baseApp.ValidateCreateItemCompra(item);

                    if ((Int32)Session["IdVolta"] == 0)
                    {
                        return RedirectToAction("IncluirItemPedidoCompra");
                    }

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoPedidoCompra");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    ModelState.AddModelError("", ex.Message);
                    return RedirectToAction("VoltarAnexoPedidoCompra");
                }
            }
            else
            {
                return RedirectToAction("VoltarAnexoPedidoCompra");
            }
        }

        [HttpPost]
        public JsonResult GetTemplate()
        {
            var result = new Hashtable();
            result.Add("TEMP_TX_CORPO", tempApp.GetByCode("COTFORN").TEMP_TX_CORPO);
            return Json(result);
        }

        [HttpPost]
        public void EnviarCotacaoFornecedor(Int32 forn, Int32 pedido)
        {
            var prod = baseApp.GetItemById(pedido).ITEM_PEDIDO_COMPRA.Where(x => x.PROD_CD_ID != null).SelectMany(x => x.PRODUTO.PRODUTO_FORNECEDOR).Where(x => x.PRFO_IN_ATIVO == 1).Select(x => new { FORN_CD_ID = x.FORN_CD_ID, PROD_CD_ID = x.PROD_CD_ID });
            prod = prod.Where(x => x.FORN_CD_ID == forn);
            if (prod != null && prod.Count() != 0)
            {
                foreach (var p in prod)
                {
                    if (Session["EnviarCotacaoFornProd"] == null)
                    {
                        Session["EnviarCotacaoFornProd"] = new List<PRODUTO_FORNECEDOR>();
                    }
                    List<PRODUTO_FORNECEDOR> lista = (List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"];
                    lista.Add(proApp.GetByProdForn(p.FORN_CD_ID, p.PROD_CD_ID));
                    Session["EnviarCotacaoFornProd"] = lista;
                }
            }
        }

        [HttpPost]
        public void RemoverCotacaoFornecedor(Int32 forn)
        {
            try
            {
                List<PRODUTO_FORNECEDOR> lista = (List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"];
                lista.RemoveAll(x => x.FORN_CD_ID == forn);
                Session["EnviarCotacaoFornProd"] = lista;

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
        }

        [HttpGet]
        public ActionResult EnviarCotacaoPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            List<FORNECEDOR> forn = new List<FORNECEDOR>();
            foreach (var itpc in item.ITEM_PEDIDO_COMPRA)
            {                
                if (itpc.PRODUTO != null)
                {
                    forn.AddRange(itpc.PRODUTO.PRODUTO_FORNECEDOR.Where(x => x.PRFO_IN_ATIVO == 1).Select(x => x.FORNECEDOR));
                }
            }
            Session["ITPCForn"] = forn.Distinct().ToList<FORNECEDOR>();
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["ITPCForn"];
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 4;
            Session["IdVolta"] = id;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EnviarCotacaoPedidoCompra(PedidoCompraViewModel vm, Int32? enviaSms, String emailPers)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<FORNECEDOR> forn = (List<FORNECEDOR>)Session["ITPCForn"];
            ViewBag.Fornecedores = (List<FORNECEDOR>)Session["ITPCForn"];
            try
            {
                List<AttachmentForn> attachmentForn = new List<AttachmentForn>();
                String voltaSms = String.Empty;

                // Executa a operação
                PEDIDO_COMPRA ped = baseApp.GetItemById(vm.PECO_CD_ID);
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);

                if (voltaSms == "1")
                {
                    Session["MensMensagem"] = 1;
                    ModelState.AddModelError("", "Fornecedor Inexistente");
                    return View(vm);
                }

                if (voltaSms == "2")
                {
                    Session["MensMensagem"] = 1;
                    ModelState.AddModelError("", "Fornecedor não possuí celular cadastrado");
                    return View(vm);
                }

                if (voltaSms == "3")
                {
                    Session["MensMensagem"] = 1;
                    ModelState.AddModelError("", "SMS não enviado");
                    return View(vm);
                }

                if (baseApp.GetById(vm.PECO_CD_ID).ITEM_PEDIDO_COMPRA.Count == 0)
                {
                    ModelState.AddModelError("", "Pedido de compra não possui itens");
                    return View(vm);
                }

                if ((Session["EnviarCotacaoFornProd"] != null && ((List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"]).Count != 0))
                {
                    List<Int32> fornecedores = new List<Int32>();

                    if (Session["EnviarCotacaoFornProd"] != null && ((List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"]).Count != 0)
                    {
                        List<PRODUTO_FORNECEDOR> lista = (List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"];
                        fornecedores.AddRange(lista.Select(x => x.FORN_CD_ID).ToList());
                    }

                    Session["EnviarCotacaoFornProd"] = null;
                    Session["ListaEmailForn"] = fornecedores.Distinct().ToList();
                }
                else
                {
                    List<Int32> fornecedores = ped.ITEM_PEDIDO_COMPRA.Where(x => x.PRODUTO != null).SelectMany(x => x.PRODUTO.PRODUTO_FORNECEDOR.Where(y => y.PRFO_IN_ATIVO == 1)).Select(x => x.FORN_CD_ID).ToList();
                    Session["ListaEmailForn"] = fornecedores.Distinct().ToList();
                }

                if (enviaSms == 1)
                {
                    foreach (var f in (List<Int32>)Session["ListaEmailForn"])
                    {
                        var fornSms = forApp.GetItemById(f);
                        Session["PedCompra"] = ped;
                        voltaSms = EnviaSmsCotacao(fornSms);
                    }                    
                }

                if ((Session["EnviarCotacaoFornProd"] != null && ((List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"]).Count != 0))
                {
                    if (Session["EnviarCotacaoFornProd"] != null && ((List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"]).Count != 0)
                    {
                        List<PRODUTO_FORNECEDOR> lista = (List<PRODUTO_FORNECEDOR>)Session["EnviarCotacaoFornProd"];
                        var prodFornecedores = lista.GroupBy(x => x.FORN_CD_ID);
                        attachmentForn.AddRange(GeraAnexoProd(vm, ped, prodFornecedores));
                    }

                    Session["EnviarCotacaoFornProd"] = null;
                }
                else
                {
                    var prodFornecedores = ped.ITEM_PEDIDO_COMPRA.Where(x => x.PRODUTO != null).SelectMany(x => x.PRODUTO.PRODUTO_FORNECEDOR.Where(y => y.PRFO_IN_ATIVO == 1)).GroupBy(x => x.FORN_CD_ID);
                    attachmentForn.AddRange(GeraAnexoProd(vm, ped, prodFornecedores));
                }

                Int32 volta = baseApp.ValidateEnvioCotacao(item, attachmentForn, emailPers, usuario, forn);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCompra"] = 1;
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0102", CultureInfo.CurrentCulture));
                    return View(item);
                }

                // Sucesso
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                if (enviaSms != 1)
                {
                    Session["SmsEmailEnvio"] = 2;
                }
                else
                {
                    Session["SmsEmailEnvio"] = 1;
                }
                Session["IdCompra"] = item.PECO_CD_ID;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public List<AttachmentForn> GeraAnexoProd(PedidoCompraViewModel vm, PEDIDO_COMPRA ped, IEnumerable<IGrouping<Int32, PRODUTO_FORNECEDOR>> prodFornecedores)
        {
            List<AttachmentForn> list = new List<AttachmentForn>();

            foreach (var group in prodFornecedores)
            {
                Int32 groupKey = group.Key;

                FORNECEDOR forn = forApp.GetItemById(groupKey);

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = "Itens - Pedido de Compra " + vm.PECO_CD_ID;
                List<ITEM_PEDIDO_COMPRA> lista = new List<ITEM_PEDIDO_COMPRA>();

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                MemoryStream ms = new MemoryStream();
                //PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(titulo, meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 4;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 100f, 100f, 150f, 80f, 40f, 100f, 100f, 40f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Itens de Pedido - " + forn.FORN_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 9;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Fabricante", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (var groupedItem in group)
                {
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ped.ITEM_PEDIDO_COMPRA.FirstOrDefault(x => x.PROD_CD_ID == groupedItem.PROD_CD_ID).ITPC_TX_OBSERVACOES, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(groupedItem.PRODUTO.PROD_NM_FABRICANTE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(ped.ITEM_PEDIDO_COMPRA.FirstOrDefault(x => x.PROD_CD_ID == groupedItem.PROD_CD_ID).ITPC_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza Pdf
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                ms.Position = 0;
                Attachment anexo = new Attachment(ms, "ItensPedido" + ped.PECO_CD_ID + ".pdf");
                AttachmentForn af = new AttachmentForn();
                af.FORN_CD_ID = groupKey;
                af.ATTACHMENT = anexo;
                list.Add(af);
            }
            return list;
        }

        [HttpPost]
        public String EnviaSmsCotacao(FORNECEDOR item)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            PEDIDO_COMPRA ped = (PEDIDO_COMPRA)Session["PedCompra"];
            String volta = baseApp.ValidateCreateMensagem(item, usuarioLogado, ped, idAss);
            return volta;
        }

        [HttpGet]
        public ActionResult ProcessarEnviarAprovacaoPedidoCompra(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PEDIDO_COMPRA item = baseApp.GetItemById(id);
            Decimal custo = 0;
            foreach (var i in item.ITEM_PEDIDO_COMPRA)
            {
                if (i.ITPC_VL_PRECO_SELECIONADO != null)
                {
                    custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                }
            }
            ViewBag.CustoTotal = custo;
            PedidoCompraViewModel vm = Mapper.Map<PEDIDO_COMPRA, PedidoCompraViewModel>(item);
            Session["IdVolta"] = id;
            Session["MensCompra"] = 0;
            Session["VoltaCompra"] = 8;
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ProcessarEnviarAprovacaoPedidoCompra(PedidoCompraViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                PEDIDO_COMPRA item = Mapper.Map<PedidoCompraViewModel, PEDIDO_COMPRA>(vm);
                Decimal custo = 0;
                foreach (var i in item.ITEM_PEDIDO_COMPRA)
                {
                    if (i.ITPC_VL_PRECO_SELECIONADO != null)
                    {
                        custo += (Int32)i.ITPC_NR_QUANTIDADE_REVISADA * (Decimal)i.ITPC_VL_PRECO_SELECIONADO;
                    }
                }
                ViewBag.CustoTotal = custo;
                Int32 volta = baseApp.ValidateEnvioAprovacao(item);

                // Sucesso
                Session["MensCompra"] = 0;
                listaMaster = new List<PEDIDO_COMPRA>();
                Session["ListaCompra"] = null;
                Session["IdCompra"] = item.PECO_CD_ID;
                return RedirectToAction("MontarTelaPedidoCompra");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult IncluirAcompanhamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PEDIDO_COMPRA_ACOMPANHAMENTO coment = new PEDIDO_COMPRA_ACOMPANHAMENTO();
            PedidoCompraAcompanhamentoViewModel vm = Mapper.Map<PEDIDO_COMPRA_ACOMPANHAMENTO, PedidoCompraAcompanhamentoViewModel>(coment);
            Session["IdVolta"] = id;
            vm.PCAT_DT_ACOMPANHAMENTO = DateTime.Now;
            vm.PCAT_IN_ATIVO = 1;
            vm.PECO_CD_ID = id;
            vm.USUARIO = usuario;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAcompanhamento(PedidoCompraAcompanhamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            Session["VoltaAcompanhamento"] = true;

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PEDIDO_COMPRA_ACOMPANHAMENTO item = Mapper.Map<PedidoCompraAcompanhamentoViewModel, PEDIDO_COMPRA_ACOMPANHAMENTO>(vm);
                    Int32 volta = baseApp.ValidateCreateAcompanhamento(item);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("VoltarAnexoPedidoCompra");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardCompra()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if ((Int32)Session["PermCompra"] == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaProdutoDash"] = 6;
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);
            return View(vm);
        }

    }
}