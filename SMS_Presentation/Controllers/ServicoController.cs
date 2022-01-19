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

namespace ERP_CRM_Solution.Controllers
{
    public class ServicoController : Controller
    {
        private readonly IServicoAppService servApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaServicoAppService csApp;
        private readonly IFilialAppService filApp;
        //private readonly IPedidoVendaAppService pedvApp;
        private readonly IServicoTabelaPrecoAppService stbApp;

        private String msg;
        private Exception exception;
        SERVICO objetoServ = new SERVICO();
        SERVICO objetoServAntes = new SERVICO();
        List<SERVICO> listaMasterServ = new List<SERVICO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public ServicoController(IServicoAppService servApps, ILogAppService logApps, IUnidadeAppService unApps, ICategoriaServicoAppService csApps, IFilialAppService filApps, IServicoTabelaPrecoAppService stbApps)
        {
            servApp = servApps;
            logApp = logApps;
            unApp = unApps;
            csApp = csApps;
            filApp = filApps;
            //pedvApp = pedvApps;
            stbApp = stbApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
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

        [HttpPost]
        public JsonResult BuscaNome(String nome)
        {
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SERVICO> lista = servApp.GetAllItens(idAss);
            Session["Servicos"] = lista;
            if (nome != null)
            {
                List<SERVICO> lstProduto = lista.Where(x => x.SERV_NM_NOME != null && x.SERV_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<SERVICO>();

                if (lstProduto != null)
                {
                    foreach (var item in lstProduto)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.SERV_CD_ID);
                        result.Add("text", item.SERV_NM_NOME);
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaServico()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            if (Session["ListaServico"] == null)
            {
                listaMasterServ = servApp.GetAllItens(idAss);
                Session["ListaServico"] = listaMasterServ;
            }
            ViewBag.Listas = (List<SERVICO>)Session["ListaServico"];
            ViewBag.Title = "Serviços";
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Servicos = ((List<SERVICO>)Session["ListaServico"]).Count;

            if (Session["MensServico"] != null)
            {
                if ((Int32)Session["MensServico"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0121", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0122", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 50)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0124", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensServico"] = 0;
            objetoServ = new SERVICO();
            return View(objetoServ);
        }

        public ActionResult RetirarFiltroServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaServico"] = null;
            Session["FiltroServico"] = null;
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult MostrarTudoServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterServ = servApp.GetAllItensAdm(idAss);
            Session["FiltroServico"] = null;
            Session["ListaServico"] = listaMasterServ;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpPost]
        public ActionResult FiltrarServico(SERVICO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<SERVICO> listaObj = new List<SERVICO>();
                Session["FiltroServico"] = item;
                Int32 volta = servApp.ExecuteFilter(item.CASE_CD_ID, item.SERV_NM_NOME, item.SERV_DS_DESCRICAO, item.SERV_TX_OBSERVACOES, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaServico");
                }

                // Sucesso
                listaMasterServ = listaObj;
                Session["ListaServico"] = listaObj;
                return RedirectToAction("MontarTelaServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaProduto");
            }
        }

        public ActionResult VoltarBaseServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult IncluirCategoriaServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("IncluirCategoriaServico", "TabelasAuxiliares");
        }

        [HttpGet]
        public ActionResult IncluirServico()
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

            // Verifica possibilidade
            Int32 num = servApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumServicos"] <= num)
            {
                Session["MensServico"] = 50;
                return RedirectToAction("MontarTelaServico");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            Session["VoltaPop"] = 2;
            SERVICO item = new SERVICO();
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SERV_DT_CADASTRO = DateTime.Today.Date;
            vm.SERV_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public void MontarListaCusto(SERVICO_TABELA_PRECO item)
        {
            Task.Run(() => {
                List<SERVICO_TABELA_PRECO> listaPr = new List<SERVICO_TABELA_PRECO>();
                if (Session["ListaPrecoServico"] == null)
                {                    
                    listaPr = new List<SERVICO_TABELA_PRECO>();
                }
                listaPr = (List<SERVICO_TABELA_PRECO>)Session["ListaPrecoServico"];
                item.SETP_IN_ATIVO = 1;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Now;
                listaPr.Add(item);
                Session["ListaPrecoServico"] = listaPr;
            });
        }

        [HttpPost]
        public void RemovePrecoTabela(SERVICO_TABELA_PRECO item)
        {
            Task.Run(() => {
                if (Session["ListaPrecoServico"] != null)
                {
                    List<SERVICO_TABELA_PRECO> lista = (List<SERVICO_TABELA_PRECO>)Session["ListaPrecoServico"];
                    lista.RemoveAll(x => x.FILI_CD_ID == item.FILI_CD_ID);
                    Session["ListaPrecoServico"] = lista;
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirServico(ServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = servApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensServico"] = 3;
                        return RedirectToAction("MontarTelaServico");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.SERV_CD_ID;
                    if (Session["FileQueueServico"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueServico"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueServico(file);
                        }

                        Session["FileQueueServico"] = null;
                    }

                    if (Session["ListaPrecoServico"] != null)
                    {
                        IncluirTabelaServico(item.SERV_CD_ID);
                    }

                    // Sucesso
                    listaMasterServ = new List<SERVICO>();
                    Session["ListaServico"] = null;
                    return RedirectToAction("MontarTelaServico");
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

        public void IncluirTabelaServico(Int32 id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SERVICO_TABELA_PRECO> lista = (List<SERVICO_TABELA_PRECO>)Session["ListaPrecoServico"];
            lista.Select(c => { c.SERV_CD_ID = id; return c; }).ToList<SERVICO_TABELA_PRECO>();

            Int32 volta = stbApp.ValidateCreateLista(lista, idAss);
            Session["ListaPrecoServico"] = null;
        }

        public ActionResult IncluirItemTabelaServico(SERVICO_TABELA_PRECO item)
        {
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                item.SETP_IN_ATIVO = 1;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Now;

                Int32 volta = stbApp.ValidateCreate(item, idAss);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensPreco"] = 1;
                }

                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
        }

        [HttpPost]
        public ActionResult EditarPC(SERVICO_TABELA_PRECO item, Int32 id)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];;

                item.SERV_CD_ID = id;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Today.Date;
                item.SETP_IN_ATIVO = 1;

                Int32 volta = stbApp.ValidateEdit(item, id);

                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("EditarProduto", new { id = item.SERV_CD_ID });
            }
        }

        [HttpGet]
        public ActionResult ExcluirTabelaServico(Int32 id)
        {

            // Verifica se tem usuario logado
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            SERVICO_TABELA_PRECO itemAntes = stbApp.GetById(id);
            SERVICO_TABELA_PRECO item = new SERVICO_TABELA_PRECO();
            item.FILI_CD_ID = itemAntes.FILI_CD_ID;
            item.SERV_CD_ID = itemAntes.SERV_CD_ID;
            item.SETP_CD_ID = itemAntes.SETP_CD_ID;
            item.SETP_DT_DATA_REAJUSTE = itemAntes.SETP_DT_DATA_REAJUSTE;
            item.SETP_IN_ATIVO = 0;
            item.SETP_NR_MARKUP = itemAntes.SETP_NR_MARKUP;
            item.SETP_VL_CUSTO = itemAntes.SETP_VL_CUSTO;
            item.SETP_VL_PRECO = itemAntes.SETP_VL_PRECO;
            item.SETP_VL_PRECO_PROMOCAO = itemAntes.SETP_VL_PRECO_PROMOCAO;
            item.SETP_VL_DESCONTO_MAXIMO = itemAntes.SETP_VL_DESCONTO_MAXIMO;
            Int32 volta = stbApp.ValidateDelete(item, id);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult ReativarTabelaServico(Int32 id)
        {

            // Verifica se tem usuario logado
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            SERVICO_TABELA_PRECO itemAntes = stbApp.GetById(id);
            SERVICO_TABELA_PRECO item = new SERVICO_TABELA_PRECO();
            item.FILI_CD_ID = itemAntes.FILI_CD_ID;
            item.SERV_CD_ID = itemAntes.SERV_CD_ID;
            item.SETP_CD_ID = itemAntes.SETP_CD_ID;
            item.SETP_DT_DATA_REAJUSTE = itemAntes.SETP_DT_DATA_REAJUSTE;
            item.SETP_IN_ATIVO = 1;
            item.SETP_NR_MARKUP = itemAntes.SETP_NR_MARKUP;
            item.SETP_VL_CUSTO = itemAntes.SETP_VL_CUSTO;
            item.SETP_VL_PRECO = itemAntes.SETP_VL_PRECO;
            item.SETP_VL_PRECO_PROMOCAO = itemAntes.SETP_VL_PRECO_PROMOCAO;
            item.SETP_VL_DESCONTO_MAXIMO = itemAntes.SETP_VL_DESCONTO_MAXIMO;
            Int32 volta = stbApp.ValidateReativar(item, id);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult VerServico(Int32 id)
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            //ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoServAntes = item;
            Session["Servico"] = item;
            Session["IdVolta"] = id;
            Session["VoltaPop"] = 3;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarServico(Int32 id)
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

            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            //ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoServAntes = item;

            if (Session["MensPreco"] != null)
            {
                if ((Int32)Session["MensPreco"] == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                    Session["MensPreco"] = 0;
                }
                if ((Int32)Session["MensServico"] == 30)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensServico"] == 31)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            Session["Servico"] = item;
            Session["IdVolta"] = id;
            Session["VoltaPop"] = 3;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarServico(ServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(idAss).OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(p => p.UNID_IN_TIPO_UNIDADE == 2).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(p => p.NBSE_NM_NOME), "NBSE_CD_ID", "NBSE_NM_NOME");
            List<SelectListItem> local = new List<SelectListItem>();
            local.Add(new SelectListItem() { Text = "Interno", Value = "1" });
            local.Add(new SelectListItem() { Text = "Externo", Value = "2" });
            local.Add(new SelectListItem() { Text = "Interno/externo", Value = "3" });
            ViewBag.Local = new SelectList(local, "Value", "Text");
            //ViewBag.LstPedidos = pedvApp.GetAllItens();
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                    Int32 volta = servApp.ValidateEdit(item, objetoServAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterServ = new List<SERVICO>();
                    Session["ListaServico"] = null;
                    return RedirectToAction("MontarTelaServico");
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
        public ActionResult ExcluirServico(Int32 id)
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

            // Executar
            SERVICO item = servApp.GetItemById(id);
            objetoServAntes = item;
            item.SERV_IN_ATIVO = 0;
            Int32 volta = servApp.ValidateEdit(item, item, usuario);
            if (volta == 1)
            {
                Session["MensServico"] = 4;
                return RedirectToAction("MontarTelaServico");
            }
            listaMasterServ = new List<SERVICO>();
            Session["ListaServico"] = null;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult ReativarServico(Int32 id)
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

            // Verifica possibilidade
            Int32 num = servApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumServicos"] <= num)
            {
                Session["MensServico"] = 50;
                return RedirectToAction("MontarTelaServico");
            }

            // Executar
            SERVICO item = servApp.GetItemById(id);
            item.SERV_IN_ATIVO = 1;
            objetoServAntes = item;
            Int32 volta = servApp.ValidateReativar(item, usuario);
            listaMasterServ = new List<SERVICO>();
            Session["ListaServico"] = null;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult VerAnexoServico(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarServico", new { id = (Int32)Session["IdVolta"] });
        }

        public FileResult DownloadServico(Int32 id)
        {
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            String arquivo = item.SEAN_AQ_ARQUIVO;
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

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
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

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }

            Session["FileQueueServico"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueServico(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (file == null)
            {
                Session["MensServico"] = 30;
                return RedirectToAction("VoltarAnexoServico");
            }

            SERVICO item = servApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                Session["MensServico"] = 31;
                return RedirectToAction("VoltarAnexoServico");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SERVICO_ANEXO foto = new SERVICO_ANEXO();
            foto.SEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SEAN_DT_ANEXO = DateTime.Today;
            foto.SEAN_IN_ATIVO = 1;
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
            foto.SEAN_IN_TIPO = tipo;
            foto.SEAN_NM_TITULO = fileName;
            foto.SERV_CD_ID = item.SERV_CD_ID;

            item.SERVICO_ANEXO.Add(foto);
            objetoServAntes = item;
            Int32 volta = servApp.ValidateEdit(item, objetoServAntes);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpPost]
        public ActionResult UploadFileServico(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (file == null)
            {
                Session["MensServico"] = 30;
                return RedirectToAction("VoltarAnexoServico");
            }

            SERVICO item = servApp.GetById((Int32)Session["IdVolta"]);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensServico"] = 31;
                return RedirectToAction("VoltarAnexoServico");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SERVICO_ANEXO foto = new SERVICO_ANEXO();
            foto.SEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SEAN_DT_ANEXO = DateTime.Today;
            foto.SEAN_IN_ATIVO = 1;
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
            foto.SEAN_IN_TIPO = tipo;
            foto.SEAN_NM_TITULO = fileName;
            foto.SERV_CD_ID = item.SERV_CD_ID;

            item.SERVICO_ANEXO.Add(foto);
            objetoServAntes = item;
            Int32 volta = servApp.ValidateEdit(item, objetoServAntes);
            return RedirectToAction("VoltarAnexoServico");
        }

        public ActionResult GerarRelatorioLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ServicoLista" + "_" + data + ".pdf";
            List<SERVICO> lista = (List<SERVICO>)Session["ListaServico"];
            SERVICO filtro = (SERVICO)Session["FiltroServico"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
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

            cell = new PdfPCell(new Paragraph("Serviços - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 70f, 160f, 300f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Serviços selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 4;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (SERVICO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_SERVICO.CASE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_CD_CODIGO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CASE_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CASE_CD_ID;
                    ja = 1;
                }
                if (filtro.SERV_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.SERV_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.SERV_NM_NOME;
                    }
                }
                if (filtro.SERV_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.SERV_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.SERV_DS_DESCRICAO;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            SERVICO serv = servApp.GetById((Int32)Session["IdVolta"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Serviço" + serv.SERV_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

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

            cell = new PdfPCell(new Paragraph("Serviço - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (serv.CATEGORIA_SERVICO != null)
            {
                cell = new PdfPCell(new Paragraph("Categoria: " + serv.CATEGORIA_SERVICO.CASE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Categoria: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Filial: " + serv.FILIAL.FILI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (serv.UNIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Unidade: " + serv.UNIDADE.UNID_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Unidade: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (serv.NOMENCLATURA_BRAS_SERVICOS != null)
            {
                cell = new PdfPCell(new Paragraph("Cod.NBS: " + serv.NOMENCLATURA_BRAS_SERVICOS.NBSE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Cod.NBS: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Código: " + serv.SERV_CD_CODIGO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + serv.SERV_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Preço: " + serv.SERV_VL_PRECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            //Descirição
            Chunk chunk = new Chunk("Descrição: " + serv.SERV_DS_DESCRICAO, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + serv.SERV_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardServiceDesk()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            return View(vm);
        }

    }
}