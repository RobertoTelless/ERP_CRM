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
    public class ProdutoController : Controller
    {
        private readonly IProdutoAppService prodApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly IFilialAppService filApp;
        private readonly IFornecedorAppService fornApp;
        private readonly IProdutotabelaPrecoAppService tpApp;
        private readonly ISubcategoriaProdutoAppService scpApp;
        //private readonly IPedidoVendaAppService pvApp;
        private readonly IProdutoEstoqueFilialAppService pefApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IFichaTecnicaAppService ftApp;

        private String msg;
        private Exception exception;
        PRODUTO objetoProd = new PRODUTO();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public ProdutoController(
            IProdutoAppService prodApps
            , ILogAppService logApps
            , IUnidadeAppService unApps
            , ICategoriaProdutoAppService cpApps
            , IFilialAppService filApps
            , IFornecedorAppService fornApps
            , IProdutotabelaPrecoAppService tpApps
            , ISubcategoriaProdutoAppService scpApps
            , IConfiguracaoAppService confApps
            , IFichaTecnicaAppService ftApps
            //, IPedidoVendaAppService pvApps
            , IProdutoEstoqueFilialAppService pefApps)
        {
            prodApp = prodApps;
            logApp = logApps;
            unApp = unApps;
            cpApp = cpApps;
            filApp = filApps;
            fornApp = fornApps;
            tpApp = tpApps;
            scpApp = scpApps;
            //pvApp = pvApps;
            pefApp = pefApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            PRODUTO item = new PRODUTO();
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
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

        public void FlagContinua()
        {
            Session["VoltaCliente"] = 3;
        }

        public ActionResult IncluirCategoriaProduto()
        {
            Session["CategoriaToProduto"] = true;
            return RedirectToAction("IncluirCatProduto", "TabelasAuxiliares");
        }

        public ActionResult IncluirSubCategoriaProduto()
        {
            Session["SubCategoriaToProduto"] = true;
            return RedirectToAction("IncluirSubCatProduto", "TabelasAuxiliares");
        }

        [HttpPost]
        public JsonResult GetValorGrafico(Int32 id, Int32? meses)
        {
            if (meses == null)
            {
                meses = 3;
            }

            var prod = prodApp.GetById(id);

            Int32 m1 = prod.ITEM_PEDIDO_VENDA.Where(x => x.PEDIDO_VENDA.PEVE_DT_APROVACAO >= DateTime.Now.AddDays(DateTime.Now.Day * -1)).Sum(x => x.ITPE_QN_QUANTIDADE);
            Int32 m2 = prod.ITEM_PEDIDO_VENDA.Where(x => x.PEDIDO_VENDA.PEVE_DT_APROVACAO >= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-1) && x.PEDIDO_VENDA.PEVE_DT_APROVACAO <= DateTime.Now.AddDays(DateTime.Now.Day * -1)).Sum(x => x.ITPE_QN_QUANTIDADE);
            Int32 m3 = prod.ITEM_PEDIDO_VENDA.Where(x => x.PEDIDO_VENDA.PEVE_DT_APROVACAO >= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-2) && x.PEDIDO_VENDA.PEVE_DT_APROVACAO <= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-1)).Sum(x => x.ITPE_QN_QUANTIDADE);

            var hash = new Hashtable();
            hash.Add("m1", m1);
            hash.Add("m2", m2);
            hash.Add("m3", m3);

            return Json(hash);
        }

        public JsonResult GetFornecedor(Int32 id)
        {
            var forn = fornApp.GetItemById(id);
            var hash = new Hashtable();
            hash.Add("cnpj", forn.FORN_NR_CNPJ);
            hash.Add("email", forn.FORN_NM_EMAIL);
            hash.Add("tel", forn.FORN_NM_TELEFONES);
            return Json(hash);
        }

        public JsonResult VerificaFlags(Int32 id)
        {
            CATEGORIA_PRODUTO c = cpApp.GetById(id);
            var result = new Hashtable();
            result.Add("FOOD", c.CAPR_IN_FOOD == null ? 0 : c.CAPR_IN_FOOD);
            result.Add("EXP", c.CAPR_IN_EXPEDICAO == null ? 0 : c.CAPR_IN_EXPEDICAO);
            result.Add("ECOM", c.CAPR_IN_SEO == null ? 0 : c.CAPR_IN_SEO);
            result.Add("GRADE", c.CAPR_IN_GRADE == null ? 0 : c.CAPR_IN_GRADE);
            result.Add("TAMANHO", c.CAPR_IN_TAMANHO == null ? 0 : c.CAPR_IN_TAMANHO);
            return Json(result);
        }

        [HttpPost]
        public JsonResult BuscaNome(String nome)
        {
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PRODUTO> lista = prodApp.GetAllItens(idAss);

            if (nome != null)
            {
                List<PRODUTO> lstProduto = lista.Where(x => x.PROD_NM_NOME != null && x.PROD_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<PRODUTO>();

                if (lstProduto != null)
                {
                    foreach (var item in lstProduto)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.PROD_CD_ID);
                        result.Add("text", item.PROD_NM_NOME);
                        listResult.Add(result);
                    }
                }
            }
            Session["Produtos"] = lista;
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaProduto()
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
            if (Session["ListaProduto"] == null)
            {
                listaMasterProd = prodApp.GetAllItens(idAss);
                Session["ListaProduto"] = listaMasterProd;
            }
            ViewBag.Listas = (List<PRODUTO>)Session["ListaProduto"];

            if (usuario.FILIAL != null)
            {
                ViewBag.FilialUsuario = usuario.FILIAL.FILI_NM_NOME;
            }

            ViewBag.Cats = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProduto"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.CodigoProduto = Session["IdProduto"];

            // Novos indicadores
            Int32? idFilial = null;
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                idFilial = usuario.FILI_CD_ID;
            }
            List<PRODUTO_ESTOQUE_FILIAL> listaBase = prodApp.RecuperarQuantidadesFiliais(idFilial, idAss);
            List<PRODUTO_ESTOQUE_FILIAL> pontoPedido = listaBase.Where(x => x.PREF_QN_ESTOQUE < x.PRODUTO.PROD_QN_QUANTIDADE_MINIMA).ToList();
            List<PRODUTO_ESTOQUE_FILIAL> estoqueZerado = listaBase.Where(x => x.PREF_QN_ESTOQUE == 0).ToList();
            List<PRODUTO_ESTOQUE_FILIAL> estoqueNegativo = listaBase.Where(x => x.PREF_QN_ESTOQUE < 0).ToList();

            if (usuario.PERFIL.PERF_SG_SIGLA == "ADM" || usuario.PERFIL.PERF_SG_SIGLA == "GER")
            {
                Session["PontoPedido"] = pontoPedido;
                Session["EstoqueZerado"] = estoqueZerado;
                Session["EstoqueNegativo"] = estoqueNegativo;
                ViewBag.PontoPedido = pontoPedido.Count;
                ViewBag.EstoqueZerado = estoqueZerado.Count;
                ViewBag.EstoqueNegativo = estoqueNegativo.Count;
            }
            else
            {
                pontoPedido = pontoPedido.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList<PRODUTO_ESTOQUE_FILIAL>();
                estoqueZerado = estoqueZerado.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList<PRODUTO_ESTOQUE_FILIAL>();
                estoqueNegativo = estoqueNegativo.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList<PRODUTO_ESTOQUE_FILIAL>();
                Session["PontoPedido"] = pontoPedido;
                Session["EstoqueZerado"] = estoqueZerado;
                Session["EstoqueNegativo"] = estoqueNegativo;
                ViewBag.PontoPedido = pontoPedido.Count;
                ViewBag.EstoqueZerado = estoqueZerado.Count;
                ViewBag.EstoqueNegativo = estoqueNegativo.Count;
            }

            if (Session["MensProduto"] != null)
            {
                if ((Int32)Session["MensProduto"] == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensProduto"] = 0;
                }
                if ((Int32)Session["MensProduto"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    Session["MensProduto"] = 0;
                }
                if ((Int32)Session["MensProduto"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0085", CultureInfo.CurrentCulture));
                    Session["MensProduto"] = 0;
                }
                if ((Int32)Session["MensProduto"] == 20)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0086", CultureInfo.CurrentCulture));
                    Session["MensProduto"] = 0;
                }
                if ((Int32)Session["MensProduto"] == 21)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0087", CultureInfo.CurrentCulture));
                    Session["MensProduto"] = 0;
                }
            }

            // Abre view
            objetoProd = new PRODUTO();
            objetoProd.PROD_IN_ATIVO = 1;
            Session["VoltaProduto"] = 1;
            Session["VoltaConsulta"] = 1;
            Session["VoltaEstoque"] = 0;
            Session["FlagVoltaProd"] = 1;
            Session["Clonar"] = 0;
            return View(objetoProd);
        }

        public ActionResult RetirarFiltroProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaProduto"] = null;
            Session["FiltroProduto"] = null;
            if ((Int32)Session["VoltaProduto"] == 2)
            {
                return RedirectToAction("VerCardsProduto");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult MostrarTudoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterProd = prodApp.GetAllItensAdm(idAss);
            Session["FiltroProduto"] = null;
            Session["ListaProduto"] = listaMasterProd;
            if ((Int32)Session["VoltaProduto"] == 2)
            {
                return RedirectToAction("VerCardsProduto");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        [HttpPost]
        public ActionResult FiltrarProduto(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                if (usuario.PERF_CD_ID != 1)
                {
                    item.FILI_CD_ID = usuario.FILI_CD_ID;
                }
                Int32 volta = prodApp.ExecuteFilter(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_NR_BARCODE, item.PROD_CD_CODIGO, item.FILI_CD_ID, item.PROD_IN_ATIVO, item.PROD_IN_TIPO_PRODUTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensProduto"] = 1;
                }

                // Sucesso
                listaMasterProd = listaObj;
                Session["ListaProduto"] = listaObj;
                if ((Int32)Session["VoltaProduto"] == 2)
                {
                    return RedirectToAction("VerCardsProduto");
                }
                if ((Int32)Session["VoltaConsulta"] == 2)
                {
                    return RedirectToAction("VerProdutosPontoPedido");
                }
                if ((Int32)Session["VoltaConsulta"] == 3)
                {
                    return RedirectToAction("VerProdutosEstoqueZerado");
                }
                return RedirectToAction("MontarTelaProduto");

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaProduto");
            }
        }

        [HttpPost]
        public ActionResult FiltrarEstoqueProduto(PRODUTO_ESTOQUE_FILIAL item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                // Executa a operação
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["FiltroEstoque"] = item;
                Int32? idFilial = null;
                if (item.FILI_CD_ID != 0)
                {
                    idFilial = item.FILI_CD_ID;
                }
                List<PRODUTO_ESTOQUE_FILIAL> listaObj = new List<PRODUTO_ESTOQUE_FILIAL>();
                Int32 volta = prodApp.ExecuteFilterEstoque(idFilial, item.PRODUTO.PROD_NM_NOME, item.PRODUTO.PROD_NM_MARCA, item.PRODUTO.PROD_CD_CODIGO, item.PRODUTO.PROD_NR_BARCODE, item.PRODUTO.CAPR_CD_ID, item.PRODUTO.PROD_IN_TIPO_PRODUTO.Value, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensFiltroEstoque"] = 1;
                }

                // Sucesso
                if ((Int32)Session["VoltaConsulta"] == 2)
                {
                    listaObj = listaObj.Where(x => x.PREF_QN_ESTOQUE < x.PRODUTO.PROD_QN_QUANTIDADE_MINIMA).ToList();
                    if (listaObj == null || listaObj.Count == 0)
                    {
                        Session["MensFiltroEstoque"] = 1;
                        Session["FiltroEstoque"] = null;
                    }
                    Session["PontoPedido"] = listaObj;
                    return RedirectToAction("VerProdutosPontoPedido");
                }
                if ((Int32)Session["VoltaConsulta"] == 3)
                {
                    listaObj = listaObj.Where(x => x.PREF_QN_ESTOQUE == 0).ToList();
                    if (listaObj == null || listaObj.Count == 0)
                    {
                        Session["MensFiltroEstoque"] = 1;
                        Session["FiltroEstoque"] = null;
                    }
                    Session["EstoqueZerado"] = listaObj;
                    return RedirectToAction("VerProdutosEstoqueZerado");
                }
                if ((Int32)Session["VoltaConsulta"] == 4)
                {
                    listaObj = listaObj.Where(x => x.PREF_QN_ESTOQUE < 0).ToList();
                    if (listaObj == null || listaObj.Count == 0)
                    {
                        Session["MensFiltroEstoque"] = 1;
                        Session["FiltroEstoque"] = null;
                    }
                    Session["EstoqueNegativo"] = listaObj;
                    return RedirectToAction("VerProdutosEstoqueNegativo");
                }
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaProduto");
            }
        }

        public ActionResult VoltarBaseProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaProduto"] == 4)
            {
                return RedirectToAction("IncluirPedidoCompra", "Compra");
            }
            if ((Int32)Session["Clonar"] == 1)
            {
                Session["Clonar"] = 0;
                listaMasterProd = new List<PRODUTO>();
                Session["ListaProduto"] = null;
            }

            if ((Int32)Session["VoltaEstoque"] == 1)
            {
                return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
            }

            if ((Int32)Session["VoltaConsulta"] == 1)
            {
                return RedirectToAction("MontarTelaProduto");
            }
            if ((Int32)Session["VoltaConsulta"] == 2)
            {
                return RedirectToAction("VerProdutosPontoPedido");
            }
            if ((Int32)Session["VoltaConsulta"] == 3)
            {
                return RedirectToAction("VerProdutosEstoqueZerado");
            }
            if ((Int32)Session["VoltaConsulta"] == 4)
            {
                return RedirectToAction("VerProdutosEstoqueNegativo");
            }
            if ((Int32)Session["VoltaProduto"] == 2)
            {
                Session["ListaProduto"] = null;
                return RedirectToAction("VerCardsProduto");
            }
            if ((Int32)Session["VoltaProduto"] == 5)
            {
                return RedirectToAction("MontarTelaDashboardEstoque");
            }
            Session["ListaProduto"] = null;
            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult VoltarBaseProdutoDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaProdutoDash"] == 5)
            {
                return RedirectToAction("MontarTelaDashboardEstoque", "Estoque");
            }
            if ((Int32)Session["VoltaProdutoDash"] == 6)
            {
                return RedirectToAction("MontarTelaDashboardCompra", "Compra");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        [HttpGet]
        public ActionResult IncluirProduto()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensProduto"] = 2;
                    return RedirectToAction("MontarTelaProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            Session["ListaPrecoProduto"] = null;

            // Verifica possibilidade
            Int32 num = prodApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumProduto"] <= num)
            {
                Session["MensProduto"] = 50;
                return RedirectToAction("MontarTelaProduto", "Produto");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Origens = new SelectList(prodApp.GetAllOrigens(idAss).OrderBy(p => p.PROR_NM_NOME), "PROR_CD_ID", "PROR_NM_NOME");
            List<SelectListItem> tipoProduto = new List<SelectListItem>();
            tipoProduto.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipoProduto.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.TiposProduto = new SelectList(tipoProduto, "Value", "Text");
            List<SelectListItem> tipoEmbalagem = new List<SelectListItem>();
            tipoEmbalagem.Add(new SelectListItem() { Text = "Envelope", Value = "1" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Caixa", Value = "2" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Rolo", Value = "3" });
            ViewBag.TiposEmbalagem = new SelectList(tipoEmbalagem, "Value", "Text");

            // Mensagens

            // Prepara view
            Session["VoltaCatProduto"] = 2;
            Session["VoltaSubCatProduto"] = 2;
            PRODUTO item = new PRODUTO();
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.PROD_DT_CADASTRO = DateTime.Today;
            vm.PROD_IN_ATIVO = 1;
            vm.PROD_IN_TIPO_PRODUTO = 1;
            return View(vm);
        }

        [HttpPost]
        public void MontaListaCusto(PRODUTO_TABELA_PRECO item)
        {
            Task.Run(() => {
                if (Session["ListaPrecoProduto"] == null)
                {
                    Session["ListaPrecoProduto"] = new List<PRODUTO_TABELA_PRECO>();
                }
                List<PRODUTO_TABELA_PRECO> lista = (List<PRODUTO_TABELA_PRECO>)Session["ListaPrecoProduto"];
                lista.Add(item);
                Session["ListaPrecoProduto"] = lista;
            });
        }

        [HttpPost]
        public void RemovePrecoTabela(PRODUTO_TABELA_PRECO item)
        {
            Task.Run(() => {
                if (Session["ListaPrecoProduto"] != null)
                {
                    List<PRODUTO_TABELA_PRECO> lista = (List<PRODUTO_TABELA_PRECO>)Session["ListaPrecoProduto"];
                    lista.RemoveAll(x => x.FILI_CD_ID == item.FILI_CD_ID);
                    Session["ListaPrecoProduto"] = lista;
                }
            });
        }

        [HttpPost]
        public ActionResult IncluirProduto(ProdutoViewModel vm, String tabelaProduto)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Hashtable result = new Hashtable();
            vm.PROD_QN_QUANTIDADE_INICIAL = 0;

            ViewBag.Tipos = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Origens = new SelectList(prodApp.GetAllOrigens(idAss).OrderBy(p => p.PROR_NM_NOME), "PROR_CD_ID", "PROR_NM_NOME");
            List<SelectListItem> tipoProduto = new List<SelectListItem>();
            tipoProduto.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipoProduto.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.TiposProduto = new SelectList(tipoProduto, "Value", "Text");
            List<SelectListItem> tipoEmbalagem = new List<SelectListItem>();
            tipoEmbalagem.Add(new SelectListItem() { Text = "Envelope", Value = "1" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Caixa", Value = "2" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Rolo", Value = "3" });
            ViewBag.TiposEmbalagem = new SelectList(tipoEmbalagem, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    if (prodApp.CheckExist(item.PROD_NR_BARCODE, item.PROD_CD_CODIGO, idAss) != null)
                    {
                        ModelState.AddModelError("", "Código de barras ou código de produto já utilizado");
                        return View(vm);
                    }
                    Int32 volta = prodApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Acerta codigo do produto
                    if (item.PROD_CD_CODIGO == null)
                    {
                        item.PROD_CD_CODIGO = item.PROD_CD_ID.ToString();
                        volta = prodApp.ValidateEdit(item, item, usuarioLogado);
                    }

                    // Carrega foto e processa alteracao
                    item.PROD_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                    volta = prodApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/QR/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.PROD_CD_ID;
                    if (Session["FileQueueProduto"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueProduto"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueProduto(file);
                            }
                            else
                            {
                                UploadFotoQueueProduto(file);
                            }
                        }

                        Session["FileQueueProduto"] = null;
                    }

                    vm.PROD_CD_ID = item.PROD_CD_ID;
                    if (Session["ListaPrecoProduto"] != null)
                    {
                        IncluirTabelaProduto(vm, tabelaProduto);
                    }
                    Session["IdProduto"] = item.PROD_CD_ID;
                    Session["IdVolta"] = item.PROD_CD_ID;

                    // Sucesso
                    if (item.PROD_IN_COMPOSTO == 0)
                    {
                        listaMasterProd = new List<PRODUTO>();
                        Session["ListaProduto"] = null;
                        if ((Int32)Session["VoltaProduto"] == 2)
                        {
                            return RedirectToAction("VerCardsProduto");
                        }

                        if ((Int32)Session["VoltaProduto"] == 3)
                        {
                            Session["VoltaProduto"] = 0;
                            return RedirectToAction("IncluirCliente", "Cliente");
                        }
                        if ((Int32)Session["VoltaProduto"] == 4)
                        {
                            return RedirectToAction("IncluirPedidoCompra", "Compra");
                        }
                        return RedirectToAction("VoltarAnexoProduto");
                    }
                    else
                    {
                        Session["IdProduto"] = item.PROD_CD_ID;
                        Session["VoltaComposto"] = 1;
                        return RedirectToAction("IncluirFT", "FichaTecnica");
                    }
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

        public ActionResult ClonarProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara objeto
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO item = prodApp.GetItemById(id);
            PRODUTO novo = new PRODUTO();
            novo.ASSI_CD_ID = item.ASSI_CD_ID;
            novo.CAPR_CD_ID = item.CAPR_CD_ID;
            novo.FILI_CD_ID = item.FILI_CD_ID;
            novo.PROD_AQ_FOTO = item.PROD_AQ_FOTO;
            novo.PROD_CD_GTIN_EAN = item.PROD_CD_GTIN_EAN;
            novo.PROD_DS_DESCRICAO = item.PROD_DS_DESCRICAO;
            novo.PROD_DS_INFORMACAO_NUTRICIONAL = item.PROD_DS_INFORMACAO_NUTRICIONAL;
            novo.PROD_DS_INFORMACOES = item.PROD_DS_INFORMACOES;
            novo.PROD_DT_CADASTRO = DateTime.Today;
            novo.PROD_IN_ATIVO = 1;
            novo.PROD_IN_AVISA_MINIMO = item.PROD_IN_AVISA_MINIMO;
            novo.PROD_IN_BALANCA_PDV = item.PROD_IN_BALANCA_PDV;
            novo.PROD_IN_BALANCA_RETAGUARDA = item.PROD_IN_BALANCA_RETAGUARDA;
            novo.PROD_IN_COBRAR_MAIOR = item.PROD_IN_COBRAR_MAIOR;
            novo.PROD_IN_DIVISAO = item.PROD_IN_DIVISAO;
            novo.PROD_IN_GERAR_ARQUIVO = item.PROD_IN_GERAR_ARQUIVO;
            novo.PROD_IN_OPCAO_COMBO = item.PROD_IN_OPCAO_COMBO;
            novo.PROD_IN_TIPO_COMBO = item.PROD_IN_TIPO_COMBO;
            novo.PROD_IN_TIPO_EMBALAGEM = item.PROD_IN_TIPO_EMBALAGEM;
            novo.PROD_IN_TIPO_PRODUTO = item.PROD_IN_TIPO_PRODUTO;
            novo.PROD_NM_LOCALIZACAO_ESTOQUE = item.PROD_NM_LOCALIZACAO_ESTOQUE;
            novo.PROD_NM_NOME = "====== PRODUTO DUPLICADO ======";
            novo.PROD_NM_ORIGEM = item.PROD_NM_ORIGEM;
            novo.PROD_NR_ALTURA = item.PROD_NR_ALTURA;
            novo.PROD_NR_COMPRIMENTO = item.PROD_NR_COMPRIMENTO;
            novo.PROD_NR_DIAMETRO = item.PROD_NR_DIAMETRO;
            novo.PROD_NR_DIAS_VALIDADE = item.PROD_NR_DIAS_VALIDADE;
            novo.PROD_NR_GARANTIA = item.PROD_NR_GARANTIA;
            novo.PROD_NR_LARGURA = item.PROD_NR_LARGURA;
            novo.PROD_NR_NCM = item.PROD_NR_NCM;
            novo.PROD_NR_REFERENCIA = item.PROD_NR_REFERENCIA;
            novo.PROD_NR_VALIDADE = item.PROD_NR_VALIDADE;
            novo.PROD_PC_MARKUP_MININO = item.PROD_PC_MARKUP_MININO;
            novo.PROD_QN_ESTOQUE = 0;
            novo.PROD_QN_PESO_BRUTO = item.PROD_QN_PESO_BRUTO;
            novo.PROD_QN_PESO_LIQUIDO = item.PROD_QN_PESO_LIQUIDO;
            novo.PROD_QN_QUANTIDADE_INICIAL = 0;
            novo.PROD_QN_QUANTIDADE_MAXIMA = 0;
            novo.PROD_QN_QUANTIDADE_MINIMA = 0;
            novo.PROD_QN_RESERVA_ESTOQUE = 0;
            novo.PROD_TX_OBSERVACOES = item.PROD_TX_OBSERVACOES;
            novo.PROD_VL_CUSTO = item.PROD_VL_CUSTO;
            novo.PROD_VL_MARKUP_PADRAO = item.PROD_VL_MARKUP_PADRAO;
            novo.PROD_VL_PRECO_MINIMO = item.PROD_VL_PRECO_MINIMO;
            novo.PROD_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_PROMOCAO;
            novo.PROD_VL_PRECO_VENDA = item.PROD_VL_PRECO_VENDA;
            novo.SCPR_CD_ID = item.SCPR_CD_ID;
            novo.UNID_CD_ID = item.UNID_CD_ID;
            novo.PROD_NR_BARCODE = item.PROD_NR_BARCODE;
            novo.PROD_QR_QRCODE = item.PROD_QR_QRCODE;

            Int32 volta = prodApp.ValidateCreateLeve(novo, usuario);
            Session["IdVolta"] = novo.PROD_CD_ID;
            Session["Clonar"] = 1;

            // Acerta codigo do produto
            novo.PROD_CD_CODIGO = novo.PROD_CD_ID.ToString();
            volta = prodApp.ValidateEdit(novo, novo, usuario);

            // Carrega foto e processa alteracao
            novo.PROD_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
            volta = prodApp.ValidateEdit(novo, novo, usuario);

            // Cria pastas
            String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + novo.PROD_CD_ID.ToString() + "/Fotos/";
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + novo.PROD_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + novo.PROD_CD_ID.ToString() + "/QR/";
            Directory.CreateDirectory(Server.MapPath(caminho));
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult EditarProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensProduto"] = 2;
                    return RedirectToAction("MontarTelaProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (Session["TabPreco"] != null)
            {
                ViewBag.TabPreco = "active";
                Session["TabPreco"] = null;
            }
            else
            {
                ViewBag.TabGeral = "active";
            }

            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            ViewBag.Tipos = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Origens = new SelectList(prodApp.GetAllOrigens(idAss).OrderBy(p => p.PROR_NM_NOME), "PROR_CD_ID", "PROR_NM_NOME");
            List<SelectListItem> tipoProduto = new List<SelectListItem>();
            tipoProduto.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipoProduto.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.TiposProduto = new SelectList(tipoProduto, "Value", "Text");
            List<SelectListItem> tipoEmbalagem = new List<SelectListItem>();
            tipoEmbalagem.Add(new SelectListItem() { Text = "Envelope", Value = "1" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Caixa", Value = "2" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Rolo", Value = "3" });
            ViewBag.TiposEmbalagem = new SelectList(tipoEmbalagem, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Int32 venda = item.ITEM_PEDIDO_VENDA == null ? 0 : item.ITEM_PEDIDO_VENDA.Where(p => p.PEDIDO_VENDA != null && p.PEDIDO_VENDA.PEVE_DT_DATA.Month == DateTime.Today.Month).Sum(m => m.ITPE_QN_QUANTIDADE);
            ViewBag.Vendas = venda;

            // Recupera preços

            // Exibe mensagem
            if (item.PROD_IN_COMPOSTO == 1 & item.FICHA_TECNICA.Count == 0)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0119", CultureInfo.CurrentCulture));
            }

            if ((Int32)Session["MensProduto"] == 2)
            {
                Session["MensProduto"] = 0;
                ModelState.AddModelError("", "Campo PREÇO obrigatorio");
            }
            if ((Int32)Session["MensProduto"] == 3)
            {
                Session["MensProduto"] = 0;
                ModelState.AddModelError("", "Campo MARKUP obrigatorio");
            }
            if ((Int32)Session["MensProduto"] == 30)
            {
                Session["MensProduto"] = 0;
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
            }

            if (usuario.FILI_CD_ID != null)
            {
                item.PROD_VL_PRECO_VENDA = item.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).Select(x => x.PRTP_VL_PRECO).FirstOrDefault();
                item.PROD_VL_PRECO_PROMOCAO = item.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).Select(x => x.PRTP_VL_PRECO_PROMOCAO).FirstOrDefault();

                ViewBag.Quantidade = pefApp.GetByProdFilial(item.PROD_CD_ID, (Int32)usuario.FILI_CD_ID).PREF_QN_ESTOQUE;
                ViewBag.EstoqueAtualizado = "Filial " + usuario.FILIAL.FILI_NM_NOME;
            }
            else
            {
                ViewBag.Quantidade = pefApp.GetByProd(item.PROD_CD_ID).Sum(x => x.PREF_QN_ESTOQUE);
                ViewBag.EstoqueAtualizado = "Todas as Filiais";
            }

            // Categoria
            ViewBag.CatExpedicao = item.CATEGORIA_PRODUTO.CAPR_IN_EXPEDICAO;
            ViewBag.CatFood = item.CATEGORIA_PRODUTO.CAPR_IN_FOOD;
            ViewBag.CatGrade = item.CATEGORIA_PRODUTO.CAPR_IN_GRADE;
            ViewBag.CatSEO = item.CATEGORIA_PRODUTO.CAPR_IN_SEO;
            ViewBag.CatTamanho = item.CATEGORIA_PRODUTO.CAPR_IN_TAMANHO;

            // Exibe
            Session["VoltaConsulta"] = 1;
            objetoProdAntes = item;
            Session["Produto"] = item;
            Session["IdVolta"] = id;
            Session["IdProduto"] = id;
            Session["VoltaFTDash"] = 10;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarProduto(ProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Origens = new SelectList(prodApp.GetAllOrigens(idAss).OrderBy(p => p.PROR_NM_NOME), "PROR_CD_ID", "PROR_NM_NOME");
            List<SelectListItem> tipoProduto = new List<SelectListItem>();
            tipoProduto.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipoProduto.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.TiposProduto = new SelectList(tipoProduto, "Value", "Text");
            List<SelectListItem> tipoEmbalagem = new List<SelectListItem>();
            tipoEmbalagem.Add(new SelectListItem() { Text = "Envelope", Value = "1" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Caixa", Value = "2" });
            tipoEmbalagem.Add(new SelectListItem() { Text = "Rolo", Value = "3" });
            ViewBag.TiposEmbalagem = new SelectList(tipoEmbalagem, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                    Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes, usuarioLogado);

                    // Verifica retorno

                    Session["IdProduto"]  = item.PROD_CD_ID;

                    // Sucesso
                    listaMasterProd = new List<PRODUTO>();
                    Session["ListaProduto"] = null;
                    if ((Int32)Session["VoltaEstoque"] == 1)
                    {
                        return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
                    }
                    if ((Int32)Session["VoltaProduto"] == 2)
                    {
                        return RedirectToAction("VerCardsProduto");
                    }
                    return RedirectToAction("MontarTelaProduto");
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
        public ActionResult ConsultarProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            //ViewBag.Tipos = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            //ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            //ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            //ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            //ViewBag.Origens = new SelectList(prodApp.GetAllOrigens(idAss).OrderBy(p => p.PROR_NM_NOME), "PROR_CD_ID", "PROR_NM_NOME");
            //List<SelectListItem> tipoProduto = new List<SelectListItem>();
            //tipoProduto.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            //tipoProduto.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            //ViewBag.TiposProduto = new SelectList(tipoProduto, "Value", "Text");
            //List<SelectListItem> tipoEmbalagem = new List<SelectListItem>();
            //tipoEmbalagem.Add(new SelectListItem() { Text = "Envelope", Value = "1" });
            //tipoEmbalagem.Add(new SelectListItem() { Text = "Caixa", Value = "2" });
            //tipoEmbalagem.Add(new SelectListItem() { Text = "Rolo", Value = "3" });
            //ViewBag.TiposEmbalagem = new SelectList(tipoEmbalagem, "Value", "Text");

            Int32 venda = item.ITEM_PEDIDO_VENDA.Where(p => p.PEDIDO_VENDA.PEVE_DT_DATA.Month == DateTime.Today.Month).ToList().Sum(m => m.ITPE_QN_QUANTIDADE);
            ViewBag.Vendas = venda;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.CdUsuario = usuario.USUA_CD_ID;

            // Recupera preços

            // Exibe mensagem
            if (item.PROD_IN_COMPOSTO == 1 & item.FICHA_TECNICA.Count == 0)
            {
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0119", CultureInfo.CurrentCulture));
            }


            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" && usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                item.PROD_VL_PRECO_VENDA = item.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).Select(x => x.PRTP_VL_PRECO).FirstOrDefault();
                item.PROD_VL_PRECO_PROMOCAO = item.PRODUTO_TABELA_PRECO.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).Select(x => x.PRTP_VL_PRECO_PROMOCAO).FirstOrDefault();
            }

            objetoProdAntes = item;
            Session["Produto"]  = item;
            Session["IdVolta"]  = id;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            if (vm.PROD_IN_TIPO_EMBALAGEM == 1)
            {
                vm.Embalagem = "Envelope";
            }
            else if (vm.PROD_IN_TIPO_EMBALAGEM == 2)
            {
                vm.Embalagem = "Caixa";
            }
            else if (vm.PROD_IN_TIPO_EMBALAGEM == 3)
            {
                vm.Embalagem = "Rolo";
            }
            else
            {
                vm.Embalagem = "-";
            }
            return View(vm);
        }

        // Filtro em cascata de subcategoria
        [HttpPost]
        public JsonResult FiltrarSubCategoriaProduto(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaSubFiltrada = prodApp.GetAllSubs(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaSubFiltrada = listaSubFiltrada.Where(x => x.CAPR_CD_ID == id).ToList();
            }
            return Json(listaSubFiltrada.Select(x => new { x.SCPR_CD_ID, x.SCPR_NM_NOME }));
        }

        [HttpPost]
        public JsonResult FiltrarCategoriaProduto(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaFiltrada = cpApp.GetAllItens(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.SUBCATEGORIA_PRODUTO.Any(s => s.SCPR_CD_ID == id)).ToList();
            }

            return Json(listaFiltrada.Select(x => new { x.CAPR_CD_ID, x.CAPR_NM_NOME }));
        }

        [HttpGet]
        public ActionResult ExcluirProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensProduto"] = 2;
                    return RedirectToAction("MontarTelaProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirProduto(ProdutoViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                Int32 volta = prodApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensProduto"] = 20;
                    return RedirectToAction("MontarTelaProduto");
                }
                if (volta == 2)
                {
                    Session["MensProduto"] = 21;
                    return RedirectToAction("MontarTelaProduto");
                }

                // Sucesso
                listaMasterProd = new List<PRODUTO>();
                Session["LIstaProduto"] = null;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN" || usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensProduto"] = 2;
                    return RedirectToAction("MontarTelaProduto");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            PRODUTO item = prodApp.GetItemById(id);
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarProduto(ProdutoViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                Int32 volta = prodApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterProd = new List<PRODUTO>();
                Session["ListaProduto"] = null;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public ActionResult VerCardsProduto()
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
            if (Session["ListaProduto"] == null)
            {
                listaMasterProd = prodApp.GetAllItens(idAss);
                Session["ListaProduto"] = listaMasterProd;
            }
            ViewBag.Listas = (List<PRODUTO>)Session["ListaProduto"];
            ViewBag.Tipos = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(idAss).Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

            // Indicadores
            ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProduto"]).Count;

            // Abre view
            objetoProd = new PRODUTO();
            Session["VoltaProduto"] = 2;
            return View(objetoProd);
        }

        [HttpGet]
        public ActionResult VerAnexoProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarProduto", new { id = (Int32)Session["IdVolta"]});
        }

        public ActionResult EditarFT()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            PRODUTO prod = (PRODUTO)Session["Produto"];
            if (prod.FICHA_TECNICA.Count > 0)
            {
                Int32 idFT = prod.FICHA_TECNICA.FirstOrDefault().FITE_CD_ID;
                return RedirectToAction("EditarFT", "FichaTecnica", new { id = idFT });
            }
            return RedirectToAction("EditarProduto");
        }

        public FileResult DownloadProduto(Int32 id)
        {
            PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
            String arquivo = item.PRAN_AQ_ARQUIVO;
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

            Session["FileQueueProduto"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueProduto(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensProduto"] = 5;
                return RedirectToAction("VoltarAnexoProduto");
            }
            Int32 idProd = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            PRODUTO item = prodApp.GetById(idProd);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                Session["MensProduto"] = 6;
                return RedirectToAction("VoltarAnexoProduto");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PRODUTO_ANEXO foto = new PRODUTO_ANEXO();
            foto.PRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PRAN_DT_ANEXO = DateTime.Today;
            foto.PRAN_IN_ATIVO = 1;
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
            foto.PRAN_IN_TIPO = tipo;
            foto.PRAN_NM_TITULO = fileName;
            foto.PROD_CD_ID = item.PROD_CD_ID;

            item.PRODUTO_ANEXO.Add(foto);
            objetoProdAntes = item;
            Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpPost]
        public ActionResult UploadFileProduto(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensProduto"] = 5;
                return RedirectToAction("VoltarAnexoProduto");
            }
            Int32 idProd = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            PRODUTO item = prodApp.GetById(idProd);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensProduto"] = 6;
                return RedirectToAction("VoltarAnexoProduto");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PRODUTO_ANEXO foto = new PRODUTO_ANEXO();
            foto.PRAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PRAN_DT_ANEXO = DateTime.Today;
            foto.PRAN_IN_ATIVO = 1;
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
            foto.PRAN_IN_TIPO = tipo;
            foto.PRAN_NM_TITULO = fileName;
            foto.PROD_CD_ID = item.PROD_CD_ID;

            item.PRODUTO_ANEXO.Add(foto);
            objetoProdAntes = item;
            Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueProduto(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensProduto"] = 5;
                return RedirectToAction("VoltarAnexoProduto");
            }
            Int32 idProd = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            PRODUTO item = prodApp.GetById(idProd);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                Session["MensProduto"] = 6;
                return RedirectToAction("VoltarAnexoProduto");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                item.PROD_AQ_FOTO = "~" + caminho + fileName;
                objetoProd = item;
                Int32 volta = prodApp.ValidateEdit(item, objetoProd);
            }
            else
            {
                ViewBag.Message = SMS_Mensagens.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture);
            }
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpPost]
        public ActionResult UploadFotoProduto(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensProduto"] = 5;
                return RedirectToAction("VoltarAnexoProduto");
            }
            Int32 idProd = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            PRODUTO item = prodApp.GetById(idProd);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensProduto"] = 6;
                return RedirectToAction("VoltarAnexoProduto");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                file.SaveAs(path);

                // Gravar registro
                item.PROD_AQ_FOTO = "~" + caminho + fileName;
                objetoProd = item;
                Int32 volta = prodApp.ValidateEdit(item, objetoProd);
            }
            else
            {
                ViewBag.Message = SMS_Mensagens.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture);
            }
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpPost]
        public ActionResult UploadQRCodeProduto(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensProduto"] = 5;
                return RedirectToAction("VoltarAnexoProduto");
            }
            Int32 idProd = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            PRODUTO item = prodApp.GetById(idProd);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                Session["MensProduto"] = 6;
                return RedirectToAction("VoltarAnexoProduto");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/QR/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                file.SaveAs(path);

                // Gravar registro
                item.PROD_QR_QRCODE = "~" + caminho + fileName;
                objetoProd = item;
                Int32 volta = prodApp.ValidateEdit(item, objetoProd);
            }
            else
            {
                ViewBag.Message = SMS_Mensagens.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture);
            }
            return RedirectToAction("VoltarAnexoProduto");
        }

        public ActionResult BuscarCEPProduto(PRODUTO item)
        {
            return RedirectToAction("IncluirProdutoEspecial", new { objeto = item });
        }

        public ActionResult VerMovimentacaoEstoqueProduto()
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            PRODUTO item = prodApp.GetItemById((Int32)Session["IdVolta"]);
            objetoProdAntes = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult VerProdutosPontoPedido()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            List<PRODUTO_ESTOQUE_FILIAL> lista = (List<PRODUTO_ESTOQUE_FILIAL>)Session["PontoPedido"];

            Int32? idFilial = null;
            if (usuario.PERF_CD_ID > 2)
            {
                idFilial = usuario.FILI_CD_ID;
            }
            List<PRODUTO_ESTOQUE_FILIAL> listaBase = prodApp.RecuperarQuantidadesFiliais(idFilial, idAss);
            if (lista == null)
            {
                lista = listaBase.Where(x => x.PREF_QN_ESTOQUE < x.PRODUTO.PROD_QN_QUANTIDADE_MINIMA).ToList();
            }
            if (lista.Count == 0)
            {
                lista = listaBase.Where(x => x.PREF_QN_ESTOQUE < x.PRODUTO.PROD_QN_QUANTIDADE_MINIMA).ToList();
            }

            ViewBag.PontoPedidos = lista;
            ViewBag.PontoPedido = lista.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["PontoPedido"] = lista;

            // Abre view
            PRODUTO_ESTOQUE_FILIAL prod = new PRODUTO_ESTOQUE_FILIAL();
            Session["VoltaProduto"] = 1;
            Session["VoltaConsulta"] = 2;
            Session["Clonar"] = 0;
            Session["FiltroEstoque"] = null;
            ViewBag.Tipo = 2;
            prod.PRODUTO = new PRODUTO();
            return View(prod);
        }

        public ActionResult RetirarFiltroProdutoPontoPedido()
        {
            Session["PontoPedido"] = null;
            return RedirectToAction("VerProdutosPontoPedido");
        }

        public ActionResult RetirarFiltroProdutoEstoqueNegativo()
        {
            Session["EstoqueNegativo"] = null;
            return RedirectToAction("VerProdutosEstoqueNegativo");
        }

        public ActionResult VerProdutosEstoqueZerado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            List<PRODUTO_ESTOQUE_FILIAL> lista = (List<PRODUTO_ESTOQUE_FILIAL>)Session["EstoqueZerado"];

            Int32? idFilial = null;
            if (usuario.PERF_CD_ID != 1 && usuario.PERF_CD_ID != 2)
            {
                idFilial = usuario.FILI_CD_ID;
            }
            List<PRODUTO_ESTOQUE_FILIAL> listaBase = prodApp.RecuperarQuantidadesFiliais(idFilial, idAss);
            if (lista == null)
            {
                lista = listaBase.Where(x => x.PREF_QN_ESTOQUE == 0).ToList();
            }
            if (lista.Count == 0)
            {
                lista = listaBase.Where(x => x.PREF_QN_ESTOQUE == 0).ToList();
            }

            ViewBag.EstoqueZerados = lista;
            ViewBag.EstoqueZerado = lista.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["EstoqueZerado"] = lista;

            // Abre view
            PRODUTO_ESTOQUE_FILIAL prod = new PRODUTO_ESTOQUE_FILIAL();
            Session["VoltaProduto"] = 1;
            Session["VoltaConsulta"] = 3;
            Session["Clonar"] = 0;
            Session["FiltroEstoque"] = null;
            ViewBag.Tipo = 3;
            prod.PRODUTO = new PRODUTO();
            return View(prod);
        }

        public ActionResult VerProdutosEstoqueNegativo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(p => p.FILI_NM_NOME), "FILI_CD_ID", "FILI_NM_NOME");
            List<PRODUTO_ESTOQUE_FILIAL> lista = (List<PRODUTO_ESTOQUE_FILIAL>)Session["EstoqueNegativo"];

            Int32? idFilial = null;
            if (usuario.PERF_CD_ID > 2)
            {
                idFilial = usuario.FILI_CD_ID;
            }
            List<PRODUTO_ESTOQUE_FILIAL> listaBase = prodApp.RecuperarQuantidadesFiliais(idFilial, idAss);
            if (lista == null)
            {
                lista = listaBase.Where(x => x.PREF_QN_ESTOQUE < 0).ToList();
            }
            if (lista.Count == 0)
            {
                lista = listaBase.Where(x => x.PREF_QN_ESTOQUE < 0).ToList();
            }

            ViewBag.EstoqueNegativos = lista;
            ViewBag.EstoqueNegativo = lista.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["EstoqueNegativo"] = lista;

            // Abre view
            PRODUTO_ESTOQUE_FILIAL prod = new PRODUTO_ESTOQUE_FILIAL();
            Session["VoltaProduto"] = 1;
            Session["VoltaConsulta"] = 2;
            Session["Clonar"] = 0;
            Session["VoltaConsulta"] = 4;
            Session["FiltroEstoque"] = null;
            ViewBag.Tipo = 4;
            prod.PRODUTO = new PRODUTO();
            return View(prod);
        }

        public ActionResult RetirarFiltroProdutoEstoqueZerado()
        {
            Session["EstoqueZerado"] = null;
            return RedirectToAction("VerProdutosEstoqueZerado");
        }

        [HttpGet]
        public ActionResult EditarProdutoFornecedor(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            PRODUTO_FORNECEDOR item = prodApp.GetFornecedorById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            ProdutoFornecedorViewModel vm = Mapper.Map<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoFornecedor(ProdutoFornecedorViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    PRODUTO_FORNECEDOR item = Mapper.Map<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>(vm);
                    Int32 volta = prodApp.ValidateEditFornecedor(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoFornecedor(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_FORNECEDOR item = prodApp.GetFornecedorById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRFO_IN_ATIVO = 0;
            Int32 volta = prodApp.ValidateEditFornecedor(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ReativarProdutoFornecedor(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_FORNECEDOR item = prodApp.GetFornecedorById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRFO_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditFornecedor(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult IncluirProdutoFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            PRODUTO_FORNECEDOR item = new PRODUTO_FORNECEDOR();
            ProdutoFornecedorViewModel vm = Mapper.Map<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>(item);
            vm.PROD_CD_ID = (Int32)Session["IdVolta"];
            vm.PRFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoFornecedor(ProdutoFornecedorViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(p => p.FORN_NM_NOME), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO_FORNECEDOR item = Mapper.Map<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidateCreateFornecedor(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult EditarProdutoGrade(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(idAss).OrderBy(p => p.TAMA_NM_NOME), "TAMA_CD_ID", "TAMA_NM_NOME");
            PRODUTO_GRADE item = prodApp.GetGradeById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            ProdutoGradeViewModel vm = Mapper.Map<PRODUTO_GRADE, ProdutoGradeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoGrade(ProdutoGradeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(idAss).OrderBy(p => p.TAMA_NM_NOME), "TAMA_CD_ID", "TAMA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO_GRADE item = Mapper.Map<ProdutoGradeViewModel, PRODUTO_GRADE>(vm);
                    Int32 volta = prodApp.ValidateEditGrade(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoGrade(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PRODUTO_GRADE item = prodApp.GetGradeById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRGR_IN_ATIVO = 0;
            Int32 volta = prodApp.ValidateEditGrade(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ReativarProdutoGrade(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_GRADE item = prodApp.GetGradeById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRGR_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditGrade(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult IncluirProdutoGrade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(idAss).OrderBy(p => p.TAMA_NM_NOME), "TAMA_CD_ID", "TAMA_NM_NOME");
            PRODUTO_GRADE item = new PRODUTO_GRADE();
            ProdutoGradeViewModel vm = Mapper.Map<PRODUTO_GRADE, ProdutoGradeViewModel>(item);
            vm.PROD_CD_ID = (Int32)Session["IdVolta"];
            vm.PRGR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoGrade(ProdutoGradeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tamanhos = new SelectList(prodApp.GetAllTamanhos(idAss).OrderBy(p => p.TAMA_NM_NOME), "TAMA_CD_ID", "TAMA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO_GRADE item = Mapper.Map<ProdutoGradeViewModel, PRODUTO_GRADE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidateCreateGrade(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult EditarProdutoBarcode(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO_BARCODE item = prodApp.GetBarcodeById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            ProdutoBarcodeViewModel vm = Mapper.Map<PRODUTO_BARCODE, ProdutoBarcodeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoBarcode(ProdutoBarcodeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO_BARCODE item = Mapper.Map<ProdutoBarcodeViewModel, PRODUTO_BARCODE>(vm);
                    Int32 volta = prodApp.ValidateEditBarcode(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoBarcode(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PRODUTO_BARCODE item = prodApp.GetBarcodeById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRBC_IN_ATIVO = 0;
            Int32 volta = prodApp.ValidateEditBarcode(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ReativarProdutoBarcode(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_BARCODE item = prodApp.GetBarcodeById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRBC_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditBarcode(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult IncluirProdutoBarcode()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO_BARCODE item = new PRODUTO_BARCODE();
            ProdutoBarcodeViewModel vm = Mapper.Map<PRODUTO_BARCODE, ProdutoBarcodeViewModel>(item);
            vm.PROD_CD_ID = (Int32)Session["IdVolta"];
            vm.PRBC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoBarcode(ProdutoBarcodeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO_BARCODE item = Mapper.Map<ProdutoBarcodeViewModel, PRODUTO_BARCODE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidadeCreateBarcode(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult EditarProdutoKit(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO_KIT item = prodApp.GetKitById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            ProdutoKitViewModel vm = Mapper.Map<PRODUTO_KIT, ProdutoKitViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoKit(ProdutoKitViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO_KIT item = Mapper.Map<ProdutoKitViewModel, PRODUTO_KIT>(vm);
                    Int32 volta = prodApp.ValidateEditKit(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoKit(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            PRODUTO_KIT item = prodApp.GetKitById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRKI_IN_ATIVO = 0;
            Int32 volta = prodApp.ValidateEditKit(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ReativarProdutoKit(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_KIT item = prodApp.GetKitById(id);
            objetoProdAntes = (PRODUTO)Session["Produto"];
            item.PRKI_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditKit(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult IncluirProdutoKit()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara view
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0 & x.PROD_IN_TIPO_PRODUTO == 1 & x.PROD_IN_KIT == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            PRODUTO_KIT item = new PRODUTO_KIT();
            ProdutoKitViewModel vm = Mapper.Map<PRODUTO_KIT, ProdutoKitViewModel>(item);
            vm.PROD_CD_ID = (Int32)Session["IdVolta"];
            vm.PRKI_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoKit(ProdutoKitViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Produtos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0 & x.PROD_IN_TIPO_PRODUTO == 1 & x.PROD_IN_KIT == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PRODUTO_KIT item = Mapper.Map<ProdutoKitViewModel, PRODUTO_KIT>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidateCreateKit(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoProduto");
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

        public ActionResult GerarRelatorioFiltro()
        {
            return RedirectToAction("GerarRelatorioLista", new { id = 1 });
        }

        public ActionResult GerarRelatorioZerado()
        {
            return RedirectToAction("GerarRelatorioEstoque", new { id = 2 });
        }

        public ActionResult GerarRelatorioPonto()
        {
            return RedirectToAction("GerarRelatorioEstoque", new { id = 1 });
        }

        public ActionResult GerarRelatorioNegativo()
        {
            return RedirectToAction("GerarRelatorioEstoque", new { id = 3 });
        }

        public ActionResult GerarRelatorioLista(Int32 id)
        {
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = String.Empty;
            String titulo = String.Empty;
            List<PRODUTO> lista = new List<PRODUTO>();
            if (id == 1)
            {
                nomeRel = "ProdutoLista" + "_" + data + ".pdf";
                titulo = "Produtos - Listagem";
                lista = (List<PRODUTO>)Session["ListaProduto"];
            }
            else
            {
                return RedirectToAction("MontarTelaProduto");
            }
            PRODUTO filtro = (PRODUTO)Session["FiltroProduto"];
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

            cell = new PdfPCell(new Paragraph("Produtos selecionados pelos parametros de filtro abaixo", meuFont1))
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
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código de Barras", meuFont))
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
            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PRODUTO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_NR_BARCODE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PROD_NM_MODELO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.PROD_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.PROD_AQ_FOTO));
                    image.ScaleAbsolute(20, 20);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
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
                if (filtro.CAPR_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CAPR_CD_ID.ToString();
                    ja = 1;
                }
                if (filtro.SCPR_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Subcategoria: " + filtro.SCPR_CD_ID.ToString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Subcategoria: " + filtro.SCPR_CD_ID.ToString();
                    }
                }
                if (filtro.PROD_NR_BARCODE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Código de Barras: " + filtro.PROD_NR_BARCODE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Código de Barras: " + filtro.PROD_NR_BARCODE;
                    }
                }
                if (filtro.PROD_CD_CODIGO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Código: " + filtro.PROD_CD_CODIGO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Código: " + filtro.PROD_CD_CODIGO;
                    }
                }
                if (filtro.PROD_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.PROD_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.PROD_NM_NOME;
                    }
                }
                if (filtro.PROD_NM_MARCA != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Marca: " + filtro.PROD_NM_MARCA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Marca: " + filtro.PROD_NM_MARCA;
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

            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult GerarRelatorioEstoque(Int32 id)
        {
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = String.Empty;
            String titulo = String.Empty;
            List<PRODUTO_ESTOQUE_FILIAL> lista = new List<PRODUTO_ESTOQUE_FILIAL>();
            if (id == 1)
            {
                nomeRel = "PontoPedido" + "_" + data + ".pdf";
                titulo = "Produtos - Ponto de Pedido";
                lista = (List<PRODUTO_ESTOQUE_FILIAL>)Session["PontoPedido"];
            }
            if (id == 2)
            {
                nomeRel = "EstoqueZerado" + "_" + data + ".pdf";
                titulo = "Produtos - Estoque Zerado";
                lista = (List<PRODUTO_ESTOQUE_FILIAL>)Session["EstoqueZerado"];
            }
            if (id == 3)
            {
                nomeRel = "EstoqueNegativo" + "_" + data + ".pdf";
                titulo = "Produtos - Estoque Negativo";
                lista = (List<PRODUTO_ESTOQUE_FILIAL>)Session["EstoqueNegativo"];
            }

            PRODUTO_ESTOQUE_FILIAL filtro = (PRODUTO_ESTOQUE_FILIAL)Session["FiltroEstoque"];
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
            table = new PdfPTable(new float[] { 90f, 100f, 100f, 150f, 80f, 40f, 100f, 100f, 50f, 40f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Produtos selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 10;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Filial", meuFont))
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
            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código de Barras", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
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
            cell = new PdfPCell(new Paragraph("Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PRODUTO_ESTOQUE_FILIAL item in lista)
            {
                if (item.FILIAL != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FILIAL.FILI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NR_BARCODE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_MARCA, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_MODELO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_FABRICANTE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(Convert.ToDecimal(item.PREF_QN_ESTOQUE)), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO));
                    image.ScaleAbsolute(20, 20);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
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
                if (filtro.PRODUTO.PROD_NR_BARCODE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Código de Barras: " + filtro.PRODUTO.PROD_NR_BARCODE;
                        ja = 1;
                    }
                }
                if (filtro.FILI_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Filial: " + filtro.FILI_CD_ID.ToString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += "e Filial: " + filtro.FILI_CD_ID.ToString();
                    }
                }
                if (filtro.PRODUTO.PROD_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.PRODUTO.PROD_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.PRODUTO.PROD_NM_NOME;
                    }
                }
                if (filtro.PRODUTO.PROD_NM_MARCA != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Marca: " + filtro.PRODUTO.PROD_NM_MARCA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Marca: " + filtro.PRODUTO.PROD_NM_MARCA;
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

            if ((Int32)Session["VoltaConsulta"] == 2)
            {
                return RedirectToAction("VerProdutosPontoPedido");
            }
            if ((Int32)Session["VoltaConsulta"] == 3)
            {
                return RedirectToAction("VerProdutosEstoqueZerado");
            }
            if ((Int32)Session["VoltaConsulta"] == 4)
            {
                return RedirectToAction("VerProdutosEstoqueNegativo");
            }
            return RedirectToAction("MontarTelaProduto");
        }


        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            PRODUTO aten = prodApp.GetItemById((Int32)Session["IdVolta"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Produto_" + aten.PROD_CD_ID.ToString() + "_" + data + ".pdf";
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

            cell = new PdfPCell(new Paragraph("Produto - Detalhes", meuFont2))
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

            if (System.IO.File.Exists(Server.MapPath(aten.PROD_AQ_FOTO)))
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = Image.GetInstance(Server.MapPath(aten.PROD_AQ_FOTO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell();
            if (System.IO.File.Exists(Server.MapPath(aten.PROD_QR_QRCODE)))
            {
                cell.Border = 0;
                cell.Colspan = 1;
                image = Image.GetInstance(Server.MapPath(aten.PROD_QR_QRCODE));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Subcategoria: " + aten.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.PROD_IN_COMPOSTO == 1)
            {
                cell = new PdfPCell(new Paragraph("Composto? Sim ", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Composto? Não ", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Subcategoria: " + aten.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código de Barras: " + aten.PROD_NR_BARCODE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome: " + aten.PROD_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Marca: " + aten.PROD_NM_MARCA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Modelo: " + aten.PROD_NM_MODELO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Fabricante: " + aten.PROD_NM_FABRICANTE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Referência: " + aten.PROD_NM_REFERENCIA_FABRICANTE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.UNIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Unidade: " + aten.UNIDADE.UNID_NM_NOME, meuFont));
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
            //if (aten.PROD_VL_PRECO_VENDA != null)
            //{
            //    cell = new PdfPCell(new Paragraph("Preço de Venda: R$ " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_PRECO_VENDA.Value), meuFont));
            //    cell.Border = 0;
            //    cell.Colspan = 1;
            //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //    table.AddCell(cell);
            //}
            //else
            //{
            //    cell = new PdfPCell(new Paragraph("Preço de Venda: -", meuFont));
            //    cell.Border = 0;
            //    cell.Colspan = 1;
            //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //    table.AddCell(cell);
            //}
            //if (aten.PROD_VL_PRECO_PROMOCAO != null)
            //{
            //    cell = new PdfPCell(new Paragraph("Preço de Promõção: R$ " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_PRECO_PROMOCAO.Value), meuFont));
            //    cell.Border = 0;
            //    cell.Colspan = 1;
            //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //    table.AddCell(cell);
            //}
            //else
            //{
            //    cell = new PdfPCell(new Paragraph("Preço de Promoção: -", meuFont));
            //    cell.Border = 0;
            //    cell.Colspan = 1;
            //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //    table.AddCell(cell);
            //}
            if (aten.PROD_NR_GARANTIA != null)
            {
                cell = new PdfPCell(new Paragraph("Garantia (dias): " + aten.PROD_NR_GARANTIA.Value.ToString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Garantia (dias): ", meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Descrição: " + aten.PROD_DS_DESCRICAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Informações do Produto: " + aten.PROD_DS_INFORMACOES, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Estoque
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Estoque", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //cell = new PdfPCell(new Paragraph("Estoque Atual: " + aten.PROD_QN_ESTOQUE, meuFont));
            //cell.Border = 0;
            //cell.Colspan = 2;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque Inicial: " + aten.PROD_QN_QUANTIDADE_INICIAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque Máximo: " + aten.PROD_QN_QUANTIDADE_MAXIMA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Estoque Mínimo: " + aten.PROD_QN_QUANTIDADE_MINIMA, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Reserva de Estoque: " + aten.PROD_QN_RESERVA_ESTOQUE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.PROD_DT_ULTIMA_MOVIMENTACAO != null)
            {
                cell = new PdfPCell(new Paragraph("Último Movimento: " + aten.PROD_DT_ULTIMA_MOVIMENTACAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Lista de Fornecedores
            if (aten.PRODUTO_FORNECEDOR.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO_FORNECEDOR item in aten.PRODUTO_FORNECEDOR)
                {
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_TELEFONES, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PRFO_IN_ATIVO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Inativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Ficha Técnica
            //if (aten.PROD_IN_COMPOSTO == 1)
            //{
            //    if (aten.FICHA_TECNICA.Count > 0)
            //    {
            //        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            //        table.WidthPercentage = 100;
            //        table.HorizontalAlignment = 0;
            //        table.SpacingBefore = 1f;
            //        table.SpacingAfter = 1f;

            //        cell = new PdfPCell(new Paragraph("Ficha Técnica", meuFontBold));
            //        cell.Border = 0;
            //        cell.Colspan = 4;
            //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //        cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //        table.AddCell(cell);

            //        if (System.IO.File.Exists(Server.MapPath(aten.FICHA_TECNICA.FirstOrDefault().FITE_AQ_APRESENTACAO)))
            //        {
            //            cell = new PdfPCell();
            //            cell.Border = 0;
            //            cell.Colspan = 2;
            //            image = Image.GetInstance(Server.MapPath(aten.FICHA_TECNICA.FirstOrDefault().FITE_AQ_APRESENTACAO));
            //            image.ScaleAbsolute(100, 100);
            //            cell.AddElement(image);
            //            table.AddCell(cell);
            //        }
            //        else
            //        {
            //            cell = new PdfPCell(new Paragraph("", meuFontBold));
            //            cell.Border = 0;
            //            cell.Colspan = 1;
            //            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //            table.AddCell(cell);
            //        }
            //        cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            //        cell.Border = 0;
            //        cell.Colspan = 2;
            //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //        cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //        table.AddCell(cell);
            //        cell = new PdfPCell();

            //        cell = new PdfPCell(new Paragraph("Nome: " + aten.FICHA_TECNICA.FirstOrDefault().FITE_NM_NOME, meuFont));
            //        cell.Border = 0;
            //        cell.Colspan = 4;
            //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //        cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //        table.AddCell(cell);

            //        cell = new PdfPCell(new Paragraph("Modo de Preparo: " + aten.FICHA_TECNICA.FirstOrDefault().FITE_S_DESCRICAO, meuFont));
            //        cell.Border = 0;
            //        cell.Colspan = 4;
            //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //        cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //        table.AddCell(cell);

            //        cell = new PdfPCell(new Paragraph("Modo de Apresentação: " + aten.FICHA_TECNICA.FirstOrDefault().FITE_DS_APRESENTACAO, meuFont));
            //        cell.Border = 0;
            //        cell.Colspan = 4;
            //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //        cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //        table.AddCell(cell);
            //        pdfDoc.Add(table);

            //        if (aten.FICHA_TECNICA.FirstOrDefault().FICHA_TECNICA_DETALHE.Count > 0)
            //        {
            //            table = new PdfPTable(new float[] { 120f, 90f, 90f, 90f });
            //            table.WidthPercentage = 100;
            //            table.HorizontalAlignment = 0;
            //            table.SpacingBefore = 1f;
            //            table.SpacingAfter = 1f;

            //            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            //            {
            //                VerticalAlignment = Element.ALIGN_MIDDLE,
            //                HorizontalAlignment = Element.ALIGN_LEFT
            //            };
            //            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            table.AddCell(cell);
            //            cell = new PdfPCell(new Paragraph("Unidade", meuFont))
            //            {
            //                VerticalAlignment = Element.ALIGN_MIDDLE,
            //                HorizontalAlignment = Element.ALIGN_LEFT
            //            };
            //            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            table.AddCell(cell);
            //            cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
            //            {
            //                VerticalAlignment = Element.ALIGN_MIDDLE,
            //                HorizontalAlignment = Element.ALIGN_LEFT
            //            };
            //            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            table.AddCell(cell);
            //            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            //            {
            //                VerticalAlignment = Element.ALIGN_MIDDLE,
            //                HorizontalAlignment = Element.ALIGN_LEFT
            //            };
            //            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            table.AddCell(cell);

            //            foreach (FICHA_TECNICA_DETALHE item in aten.FICHA_TECNICA.FirstOrDefault().FICHA_TECNICA_DETALHE)
            //            {
            //                cell = new PdfPCell(new Paragraph(item.MATERIA_PRIMA.MAPR_NM_NOME, meuFont))
            //                {
            //                    VerticalAlignment = Element.ALIGN_MIDDLE,
            //                    HorizontalAlignment = Element.ALIGN_LEFT
            //                };
            //                table.AddCell(cell);
            //                cell = new PdfPCell(new Paragraph(item.MATERIA_PRIMA.UNIDADE.UNID_NM_NOME, meuFont))
            //                {
            //                    VerticalAlignment = Element.ALIGN_MIDDLE,
            //                    HorizontalAlignment = Element.ALIGN_LEFT
            //                };
            //                table.AddCell(cell);
            //                cell = new PdfPCell(new Paragraph(item.FITD_QN_QUANTIDADE.ToString(), meuFont))
            //                {
            //                    VerticalAlignment = Element.ALIGN_MIDDLE,
            //                    HorizontalAlignment = Element.ALIGN_LEFT
            //                };
            //                table.AddCell(cell);
            //                if (System.IO.File.Exists(Server.MapPath(item.MATERIA_PRIMA.MAPR_AQ_FOTO)))
            //                {
            //                    cell = new PdfPCell();
            //                    image = Image.GetInstance(Server.MapPath(item.MATERIA_PRIMA.MAPR_AQ_FOTO));
            //                    image.ScaleAbsolute(20, 20);
            //                    cell.AddElement(image);
            //                    table.AddCell(cell);
            //                }
            //                else
            //                {
            //                    cell = new PdfPCell(new Paragraph("-", meuFont))
            //                    {
            //                        VerticalAlignment = Element.ALIGN_MIDDLE,
            //                        HorizontalAlignment = Element.ALIGN_LEFT
            //                    };
            //                    table.AddCell(cell);
            //                }
            //            }
            //            pdfDoc.Add(table);
            //        }

            //    }
            //}

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            //Varejo
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Varejo", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Referência: " + aten.PROD_NR_REFERENCIA, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            if (aten.PRODUTO_GRADE.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 80f, 90f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Cor", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tamanho", meuFont))
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
                cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO_GRADE item in aten.PRODUTO_GRADE)
                {
                    cell = new PdfPCell(new Paragraph(item.PRGR_NM_COR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PRGR_NM_TAMANHO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PRGR_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PRGR_IN_ATIVO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Inativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            //Food Service
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Food Service", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.PROD_IN_BALANCA_PDV == 1)
            {
                cell = new PdfPCell(new Paragraph("Balança PDV: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Balança PDV: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.PROD_IN_BALANCA_RETAGUARDA == 1)
            {
                cell = new PdfPCell(new Paragraph("Balança Retaguarda: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Balança Retaguarda: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.PROD_IN_TIPO_COMBO == 1)
            {
                cell = new PdfPCell(new Paragraph("Produto Tipo Combo: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Produto Tipo Combo: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.PROD_IN_OPCAO_COMBO == 1)
            {
                cell = new PdfPCell(new Paragraph("Produto Opção Combo: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Produto Opção Combo: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Validade (Dias): " + aten.PROD_NR_VALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Divisão: " + aten.PROD_IN_DIVISAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.PROD_IN_COBRAR_MAIOR == 1)
            {
                cell = new PdfPCell(new Paragraph("Cobrar pelo Maior: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Cobrar pelo Maior: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.PROD_IN_GERAR_ARQUIVO == 1)
            {
                cell = new PdfPCell(new Paragraph("Gerar Arquivo Texto: Sim", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Gerar Arquivo Texto: Não", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Informações Nutricionais: " + aten.PROD_DS_INFORMACAO_NUTRICIONAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Expedição
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Expedição", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Localização no Estoque: " + aten.PROD_NM_LOCALIZACAO_ESTOQUE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Peso Líquido (Kg): " + aten.PROD_QN_PESO_LIQUIDO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Peso Bruto (Kg): " + aten.PROD_QN_PESO_BRUTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.PROD_IN_TIPO_EMBALAGEM == 1)
            {
                cell = new PdfPCell(new Paragraph("Tipo de Embalagem: Envelope", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else if (aten.PROD_IN_TIPO_EMBALAGEM == 2)
            {
                cell = new PdfPCell(new Paragraph("Tipo de Embalagem: Caixa", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Tipo de Embalagem: Rolo", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Altura (cm): " + aten.PROD_NR_ALTURA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Largura (cm): " + aten.PROD_NR_LARGURA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Comprimento (cm): " + aten.PROD_NR_COMPRIMENTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Diametro (cm): " + aten.PROD_NR_DIAMETRO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.PROD_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Movimentações 
            if (aten.MOVIMENTO_ESTOQUE_PRODUTO.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Movimentações de Estoque (Mais recentes)", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Movimentos
                table = new PdfPTable(new float[] { 80f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Filial", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (MOVIMENTO_ESTOQUE_PRODUTO item in aten.MOVIMENTO_ESTOQUE_PRODUTO.OrderByDescending(a => a.MOEP_DT_MOVIMENTO).Take(10))
                {
                    cell = new PdfPCell(new Paragraph(item.MOEP_DT_MOVIMENTO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Entrada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Saída", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    cell = new PdfPCell(new Paragraph(item.MOEP_QN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.FILIAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.FILIAL.FILI_NM_NOME.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);
            }

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpPost]
        public ActionResult IncluirTabelaProduto(ProdutoViewModel vm)
        {
            try
            {
                if (vm.PRTP_VL_PRECO == null)
                {
                    Session["TabPreco"] = 1;
                    Session["MensProduto"] = 2;
                }

                if (vm.PROD_VL_MARKUP_PADRAO == null)
                {
                    Session["TabPreco"] = 1;
                    Session["MensProduto"] = 3;
                }

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                Int32 volta = prodApp.IncluirTabelaPreco(item, usuario);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["TabPreco"] = 1;
                    Session["MensProduto"] = 30;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Sucesso
                return RedirectToAction("VoltarAnexoProduto");
            }
            catch (Exception ex)
            {
                Session["TabPreco"] = 1;
                ViewBag.Message = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("VoltarAnexoProduto");
            }
        }

        public void IncluirTabelaProduto(ProdutoViewModel vm, String tabelaProduto)
        {
            Session["VoltaConsulta"] = 1;
            PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
            List<PRODUTO_TABELA_PRECO> lista = (List<PRODUTO_TABELA_PRECO>)Session["ListaPrecoProduto"];
            foreach (var itemPreco in lista)
            {
                try
                {
                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    PRODUTO prod = new PRODUTO
                    {
                        PROD_CD_ID = item.PROD_CD_ID,
                        FILI_CD_ID = itemPreco.FILI_CD_ID,
                        PRTP_VL_PRECO = (decimal)itemPreco.PRTP_VL_PRECO,
                        PROD_VL_PRECO_PROMOCAO = (decimal)itemPreco.PRTP_VL_PRECO_PROMOCAO,
                        PROD_VL_MARKUP_PADRAO = (decimal)itemPreco.PRTP_NR_MARKUP,
                        PROD_VL_CUSTO = (decimal)itemPreco.PRTP_VL_CUSTO
                    };
                    Int32 volta = prodApp.IncluirTabelaPreco(prod, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            Session["ListaPrecoProduto"] = null;
        }

        [HttpGet]
        public ActionResult ReativarTabelaProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Verifica se tem usuario logado
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_TABELA_PRECO item = tpApp.GetItemById(id);
            item.PRTP_IN_ATIVO = 1;
            Int32 volta = prodApp.ValidateEditTabelaPreco(item);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpGet]
        public ActionResult ExcluirTabelaProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Verifica se tem usuario logado
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PRODUTO_TABELA_PRECO rl = tpApp.GetItemById(id);
            Int32 volta = tpApp.ValidateDelete(rl);
            return RedirectToAction("VoltarAnexoProduto");
        }

        [HttpPost]
        public ActionResult EditarPC(ProdutoViewModel vm, Int32 id)
        {
            try
            {
                // Executa a operação
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                PRODUTO_TABELA_PRECO prtp = new PRODUTO_TABELA_PRECO();
                PRODUTO_TABELA_PRECO prtpAntes = new PRODUTO_TABELA_PRECO();
                prtpAntes = tpApp.GetItemById(id);

                prtp.PRTP_CD_ID = id;
                prtp.PROD_CD_ID = item.PROD_CD_ID;
                prtp.FILI_CD_ID = item.FILI_CD_ID;
                prtp.PRTP_VL_CUSTO = item.PROD_VL_CUSTO;
                prtp.PRTP_NR_MARKUP = (Int32)item.PROD_VL_MARKUP_PADRAO;
                prtp.PRTP_VL_PRECO = item.PRTP_VL_PRECO;
                prtp.PRTP_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_PROMOCAO;
                prtp.PRTP_DT_DATA_REAJUSTE = DateTime.Today.Date;
                prtp.PRTP_IN_ATIVO = 1;

                Int32 volta = tpApp.ValidateEdit(prtp, prtpAntes);

                return RedirectToAction("EditarProduto", new { id = item.PROD_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("EditarProduto", new { id = vm.PROD_CD_ID });
            }
        }

        //[HttpGet]
        //public void DownloadTemplateExcel()
        //{
        //    using (ExcelPackage package = new ExcelPackage())
        //    {
        //        List<CATEGORIA_PRODUTO> lstCat = cpApp.GetAllItens();
        //        List<SUBCATEGORIA_PRODUTO> lstSub = prodApp.GetAllSubs();
        //        List<UNIDADE> lstUnidade = prodApp.GetAllUnidades();
        //        List<FILIAL> lstFilial = filApp.GetAllItens();
        //        List<PRODUTO_ORIGEM> lstOrigens = prodApp.GetAllOrigens();

        //        //PREPARA WORKSHEET PARA LISTAS
        //        ExcelWorksheet HiddenWs = package.Workbook.Worksheets.Add("Hidden");
        //        HiddenWs.Cells["A1"].LoadFromCollection(lstCat.Select(x => x.CAPR_NM_NOME));
        //        HiddenWs.Cells["B1"].LoadFromCollection(lstSub.Select(x => x.SCPR_NM_NOME));
        //        HiddenWs.Cells["C1"].LoadFromCollection(lstUnidade.Select(x => x.UNID_NM_NOME));
        //        HiddenWs.Cells["D1"].LoadFromCollection(lstFilial.Select(x => x.FILI_NM_NOME));
        //        HiddenWs.Cells["E1"].LoadFromCollection(lstOrigens.Select(x => x.PROR_NM_NOME));

        //        //PREPARA WORKSHEET DADOS GERAIS
        //        ExcelWorksheet ws1 = package.Workbook.Worksheets.Add("Dados Gerais");
        //        ws1.Cells["A1"].Value = "CATEGORIA*";
        //        ws1.Cells["B1"].Value = "SUBCATEGORIA*";
        //        ws1.Cells["C1"].Value = "TIPO DE PRODUTO*";
        //        ws1.Cells["D1"].Value = "PRODUTO COMPOSTO";
        //        ws1.Cells["E1"].Value = "CODIGO DE BARRAS";
        //        ws1.Cells["F1"].Value = "UNIDADE";
        //        ws1.Cells["G1"].Value = "NOME*";
        //        ws1.Cells["H1"].Value = "DESCRIÇÃO";
        //        ws1.Cells["I1"].Value = "INFORMAÇÕES DO PRODUTO";
        //        ws1.Cells["J1"].Value = "CÓDIGO DO PRODUTO";
        //        ws1.Cells[ws1.Dimension.Address].AutoFitColumns(13);
        //        using (ExcelRange rng = ws1.Cells["A1:J1"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            rng.Style.Locked = true;
        //        }

        //        using (ExcelRange rng = ws1.Cells["A2:J30"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //        }

        //        var listCatWS1 = ws1.DataValidations.AddListValidation("A2:A30");
        //        var listSubWS1 = ws1.DataValidations.AddListValidation("B2:B30");
        //        var listTipoWS1 = ws1.DataValidations.AddListValidation("C2:C30");
        //        var listCompostoWS1 = ws1.DataValidations.AddListValidation("D2:D30");
        //        var listUnidadeWS1 = ws1.DataValidations.AddListValidation("F2:F30");

        //        listCatWS1.Formula.ExcelFormula = "Hidden!$A$1:$A$" + lstCat.Count.ToString();
        //        listSubWS1.Formula.ExcelFormula = "Hidden!$B$1:$B$" + lstSub.Count.ToString();

        //        listTipoWS1.Formula.Values.Add("Simples");
        //        listTipoWS1.Formula.Values.Add("Kit");
        //        listTipoWS1.Formula.Values.Add("Fabricado");
        //        listTipoWS1.AllowBlank = false;

        //        listCompostoWS1.Formula.Values.Add("Sim");
        //        listCompostoWS1.Formula.Values.Add("Não");
        //        listCompostoWS1.AllowBlank = false;

        //        listUnidadeWS1.Formula.ExcelFormula = "Hidden!$C$1:$C$" + lstUnidade.Count.ToString();

        //        //PREPARA WORKSHEET ESTOQUE
        //        ExcelWorksheet ws2 = package.Workbook.Worksheets.Add("Estoque");
        //        ws2.Cells["A1"].Value = "ESTOQUE MÁXIMO";
        //        ws2.Cells["B1"].Value = "ESTOQUE MÍNIMO*";
        //        ws2.Cells["C1"].Value = "RESERVA DE ESTOQUE";
        //        ws2.Cells["D1"].Value = "AVISO DE LIMITE MÍNIMO*";
        //        ws2.Cells[ws2.Dimension.Address].AutoFitColumns(13);
        //        using (ExcelRange rng = ws2.Cells["A1:D1"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            rng.Style.Locked = true;
        //        }

        //        using (ExcelRange rng = ws2.Cells["A2:D30"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            rng.Style.Numberformat.Format = "#,##0.00";
        //        }

        //        //PREPARA WORKSHEET PREÇOS E CUSTOS
        //        ExcelWorksheet ws3 = package.Workbook.Worksheets.Add("Preços e Custos");
        //        ws3.Cells["A1"].Value = "FILIAL";
        //        ws3.Cells["B1"].Value = "CUSTO";
        //        ws3.Cells["C1"].Value = "MARKUP";
        //        ws3.Cells["D1"].Value = "PREÇO";
        //        ws3.Cells["E1"].Value = "PREÇO PROMOÇÃO";
        //        ws3.Cells[ws3.Dimension.Address].AutoFitColumns(13);
        //        using (ExcelRange rng = ws3.Cells["A1:E1"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            rng.Style.Locked = true;
        //        }

        //        using (ExcelRange rng = ws3.Cells["A2:E30"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //        }

        //        ws3.Cells["B2:E30"].Style.Numberformat.Format = "#,##0.00";

        //        var listFilWS3 = ws3.DataValidations.AddListValidation("A2:A30");
        //        listFilWS3.Formula.ExcelFormula = "Hidden!$D$1:$D$" + lstFilial.Count.ToString();

        //        //PREPARA WORKSHEET INFORMAÇÕES TRIBUTÁRIAS
        //        ExcelWorksheet ws4 = package.Workbook.Worksheets.Add("Informações Tributárias");
        //        ws4.Cells["A1"].Value = "ORIGEM";
        //        ws4.Cells["B1"].Value = "NCM";
        //        ws4.Cells["C1"].Value = "GTIN/EAN";
        //        ws4.Cells["D1"].Value = "GTIN/EAN Trib.";
        //        ws4.Cells["E1"].Value = "CEST";
        //        ws4.Cells["F1"].Value = "UNIDADE TRIBUTÁVEL";
        //        ws4.Cells["G1"].Value = "FATOR DE CONVERSÃO";
        //        ws4.Cells["H1"].Value = "COD.ENQUAD. IPI";
        //        ws4.Cells["I1"].Value = "VALOR IPI FIXO (R$)";
        //        ws4.Cells[ws4.Dimension.Address].AutoFitColumns(13);
        //        using (ExcelRange rng = ws4.Cells["A1:I1"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //            rng.Style.Locked = true;
        //        }

        //        using (ExcelRange rng = ws4.Cells["A2:I30"])
        //        {
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

        //            rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //        }

        //        var listOrigemWS4 = ws4.DataValidations.AddListValidation("A2:A30");
        //        listOrigemWS4.Formula.ExcelFormula = "Hidden!$E$1:$E$" + lstOrigens.Count.ToString();

        //        ws4.Cells["I1:I30"].Style.Numberformat.Format = "#,##0.00";

        //        HiddenWs.Hidden = eWorkSheetHidden.Hidden;
        //        Response.Clear();
        //        Response.ContentType = "application/xlsx";
        //        Response.AddHeader("content-disposition", "attachment; filename=TemplateProduto.xlsx");
        //        Response.BinaryWrite(package.GetAsByteArray());
        //        Response.End();
        //    }
        //}

        //[HttpGet]
        //public ActionResult IncluirProdutoExcel()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult IncluirProdutoExcel(HttpPostedFileBase file)
        //{
        //    USUARIO user = SessionMocks.UserCredentials;

        //    using (var pkg = new ExcelPackage(file.InputStream))
        //    {
        //        ExcelWorksheet wsGeral = pkg.Workbook.Worksheets[1];
        //        ExcelWorksheet wsEstoque = pkg.Workbook.Worksheets[2];
        //        ExcelWorksheet wsCusto = pkg.Workbook.Worksheets[3];
        //        ExcelWorksheet wsTrib = pkg.Workbook.Worksheets[4];

        //        var wsFinalRow = wsGeral.Dimension.End;

        //        for (int row = 2; row < wsFinalRow.Row; row++)
        //        {
        //            try
        //            {
        //                Int32 check = 0;
        //                PRODUTO prod = new PRODUTO();
        //                PRODUTO prodAntes = new PRODUTO();

        //                PRODUTO_TABELA_PRECO tblPreco = new PRODUTO_TABELA_PRECO();

        //                if (wsGeral.Cells[row, 5].Value != null || wsGeral.Cells[row, 10].Value != null)
        //                {
        //                    if (prodApp.CheckExist(wsGeral.Cells[row, 9].Value.ToString(), wsGeral.Cells[row, 10].Value.ToString()) != null)
        //                    {
        //                        prod = prodApp.CheckExist(wsGeral.Cells[row, 9].Value.ToString(), wsGeral.Cells[row, 10].Value.ToString());
        //                        prodAntes = prodApp.GetItemById(prod.PROD_CD_ID);
        //                        check = 1;
        //                    }
        //                }

        //                prod.ASSI_CD_ID = SessionMocks.IdAssinante;
        //                prod.PROD_DT_CADASTRO = DateTime.Now;
        //                if (wsGeral.Cells[row, 1].Value != null)
        //                {
        //                    if (cpApp.GetAllItens().Where(x => x.CAPR_NM_NOME == wsGeral.Cells[row, 1].Value.ToString()).Count() != 0)
        //                    {
        //                        prod.CAPR_CD_ID = cpApp.GetAllItens().Where(x => x.CAPR_NM_NOME == wsGeral.Cells[row, 1].Value.ToString()).First().CAPR_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        CATEGORIA_PRODUTO cp = new CATEGORIA_PRODUTO();
        //                        cp.ASSI_CD_ID = SessionMocks.IdAssinante;
        //                        cp.CAPR_IN_ATIVO = 1;
        //                        cp.CAPR_NM_NOME = wsGeral.Cells[row, 1].Value.ToString();
        //                        Int32 volta = cpApp.ValidateCreate(cp, user);

        //                        if (volta == 0)
        //                        {
        //                            prod.CAPR_CD_ID = cp.CAPR_CD_ID;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo CATEGORIA obrigatorio - Linha: " + row);
        //                    return View();
        //                }
        //                if (wsGeral.Cells[row, 2].Value != null)
        //                {
        //                    if (prodApp.GetAllSubs().Where(x => x.SCPR_NM_NOME == wsGeral.Cells[row, 2].Value.ToString()).Count() != 0)
        //                    {
        //                        prod.SCPR_CD_ID = prodApp.GetAllSubs().Where(x => x.SCPR_NM_NOME == wsGeral.Cells[row, 2].Value.ToString()).First().SCPR_CD_ID;
        //                    }
        //                    else
        //                    {
        //                        SUBCATEGORIA_PRODUTO scp = new SUBCATEGORIA_PRODUTO();
        //                        scp.ASSI_CD_ID = SessionMocks.IdAssinante;
        //                        scp.CAPR_CD_ID = prod.CAPR_CD_ID;
        //                        scp.SCPR_IN_ATIVO = 1;
        //                        scp.SCPR_NM_NOME = wsGeral.Cells[row, 2].Value.ToString();
        //                        Int32 volta = scpApp.ValidateCreate(scp, user);

        //                        if (volta == 0)
        //                        {
        //                            prod.SCPR_CD_ID = scp.CAPR_CD_ID;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo SUBCATEGORIA obrigatorio - Linha: " + row);
        //                    return View();
        //                }
        //                if (wsGeral.Cells[row, 3].Value != null)
        //                {
        //                    if (wsGeral.Cells[row, 3].Value.ToString() == "Simples")
        //                    {
        //                        prod.PROD_IN_TIPO_PRODUTO = 1;
        //                    }
        //                    else if (wsGeral.Cells[row, 3].Value.ToString() == "Kit")
        //                    {
        //                        prod.PROD_IN_TIPO_PRODUTO = 2;
        //                    }
        //                    else if (wsGeral.Cells[row, 3].Value.ToString() == "Fabricado")
        //                    {
        //                        prod.PROD_IN_TIPO_PRODUTO = 3;
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo TIPO DE PRODUTO obrigatorio - Linha: " + row);
        //                    return View();
        //                }
        //                if (wsGeral.Cells[row, 4].Value != null)
        //                {
        //                    prod.PROD_IN_COMPOSTO = wsGeral.Cells[row, 4].Value.ToString() == "Sim" ? 1 : 2;
        //                }
        //                if (wsGeral.Cells[row, 5].Value != null)
        //                {
        //                    prod.PROD_NR_BARCODE = wsGeral.Cells[row, 5].Value.ToString();
        //                }
        //                if (wsGeral.Cells[row, 6].Value != null)
        //                {
        //                    prod.UNID_CD_ID = prodApp.GetAllUnidades().Where(x => x.UNID_NM_NOME == wsGeral.Cells[row, 6].Value.ToString()).First().UNID_CD_ID;
        //                }
        //                if (wsGeral.Cells[row, 7].Value != null)
        //                {
        //                    prod.PROD_NM_NOME = wsGeral.Cells[row, 7].Value.ToString();
        //                }
        //                if (wsGeral.Cells[row, 8].Value != null)
        //                {
        //                    prod.PROD_DS_DESCRICAO = wsGeral.Cells[row, 8].Value.ToString();
        //                }
        //                if (wsGeral.Cells[row, 9].Value != null)
        //                {
        //                    prod.PROD_DS_DESCRICAO = wsGeral.Cells[row, 9].Value.ToString();
        //                }
        //                if (wsEstoque.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_QN_QUANTIDADE_MAXIMA = Convert.ToInt32(wsEstoque.Cells[row, 1].Value);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo QUANTIDADE MAXIMA obrigatorio - Linha: " + row);
        //                    return View();
        //                }
        //                if (wsEstoque.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_QN_QUANTIDADE_MINIMA = Convert.ToInt32(wsEstoque.Cells[row, 2].Value);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo QUANTIDADE MINIMA obrigatorio - Linha: " + row);
        //                    return View();
        //                }
        //                if (wsEstoque.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_QN_RESERVA_ESTOQUE = Convert.ToInt32(wsEstoque.Cells[row, 3].Value);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo QUANTIDADE ESTOQUE obrigatorio - Linha: " + row);
        //                    return View();
        //                }
        //                if (wsEstoque.Cells[row, 4].Value != null)
        //                {
        //                    prod.PROD_IN_AVISA_MINIMO = wsEstoque.Cells[row, 4].Value.ToString() == "Sim" ? 1 : 2;
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Campo AVISA DE MINIMA obrigatorio - Linha: " + row);
        //                    return View();
        //                }

        //                if (wsCusto.Cells[row, 1].Value != null)
        //                {
        //                    tblPreco.FILI_CD_ID = filApp.GetAllItens().Where(x => x.FILI_NM_NOME == wsCusto.Cells[row, 1].Value.ToString()).First().FILI_CD_ID;
        //                }
        //                if (wsCusto.Cells[row, 2].Value != null)
        //                {
        //                    tblPreco.PRTP_VL_CUSTO = Convert.ToDecimal(wsCusto.Cells[row, 2].Value);
        //                }
        //                if (wsCusto.Cells[row, 3].Value != null)
        //                {
        //                    tblPreco.PRTP_NR_MARKUP = Convert.ToInt32(wsCusto.Cells[row, 3].Value);
        //                }
        //                if (wsCusto.Cells[row, 4].Value != null)
        //                {
        //                    tblPreco.PRTP_VL_PRECO = Convert.ToDecimal(wsCusto.Cells[row, 4].Value);
        //                }
        //                if (wsCusto.Cells[row, 5].Value != null)
        //                {
        //                    tblPreco.PRTP_VL_PRECO_PROMOCAO = Convert.ToDecimal(wsCusto.Cells[row, 5].Value);
        //                }

        //                prod.PRODUTO_TABELA_PRECO.Add(tblPreco);

        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PRODUTO_ORIGEM = prodApp.GetAllOrigens().Where(x => x.PROR_NM_NOME == wsTrib.Cells[row, 1].Value.ToString()).First();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_NR_NCM = wsTrib.Cells[row, 2].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_CD_GTIN_EAN = wsTrib.Cells[row, 3].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_NR_GTIN_EAN_TRIB = wsTrib.Cells[row, 4].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_NR_CEST = wsTrib.Cells[row, 5].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_NM_UNIDADE_TRIB = wsTrib.Cells[row, 6].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_NR_FATOR_CONVERSAO = wsTrib.Cells[row, 7].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_NR_ENQUADRE_IPI = wsTrib.Cells[row, 8].Value.ToString();
        //                }
        //                if (wsTrib.Cells[row, 1].Value != null)
        //                {
        //                    prod.PROD_VL_IPI_FIXO = (decimal)wsTrib.Cells[row, 9].Value;
        //                }

        //                if (check == 0)
        //                {
        //                    Int32 volta = prodApp.ValidateCreate(prod, user);
        //                }
        //                else
        //                {
        //                    Int32 volta = prodApp.ValidateEdit(prod, prodAntes, user);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ModelState.AddModelError("", ex.Message);
        //                return View();
        //            }
        //        }
        //    }

        //    //end
        //    return RedirectToAction("MontarTelaProduto");
        //}

        //[HttpPost]
        //public ActionResult FiltrarProdutosMaisVendidos(PRODUTO item, Int32? PROD_CD_ID, DateTime? DataInicial, DateTime? DataFinal)
        //{
        //    try
        //    {
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Login", "ControleAcesso");
        //        }
        //        // Executa a operação
        //        Session["FiltroProduto"] = item;
        //        List<PRODUTOS_MAIS_VENDIDOS> lista = (List<PRODUTOS_MAIS_VENDIDOS>)Session["ListProdMaisVendidos"];

        //        if (item.CAPR_CD_ID != null)
        //        {
        //            lista = lista.Where(x => x.CAPR_CD_ID == item.CAPR_CD_ID).ToList();
        //        }
        //        if (item.SCPR_CD_ID != null)
        //        {
        //            lista = lista.Where(x => x.SCPR_CD_ID == item.SCPR_CD_ID).ToList();
        //        }
        //        if (PROD_CD_ID != null)
        //        {
        //            lista = lista.Where(x => x.PROD_CD_ID == PROD_CD_ID).ToList();
        //        }
        //        if (DataInicial != null)
        //        {
        //            lista = lista.Where(x => DbFunctions.TruncateTime(x.PRMV_ULTIMA_VENDA) >= DbFunctions.TruncateTime(DataInicial)).ToList();
        //        }
        //        if (DataFinal != null)
        //        {
        //            lista = lista.Where(x => DbFunctions.TruncateTime(x.PRMV_ULTIMA_VENDA) <= DbFunctions.TruncateTime(DataFinal)).ToList();
        //        }

        //        // Verifica retorno
        //        if (lista.Count == 0)
        //        {
        //            ModelState.AddModelError("", "Nenhum registro encontrado");
        //        }

        //        // Sucesso
        //        Session["ListProdMaisVendidos"] = lista;
        //        return RedirectToAction("MontarTelaProdutosMaisVendidos");

        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        return RedirectToAction("MontarTelaProdutosMaisVendidos");
        //    }
        //}

        //[HttpPost]
        //public ActionResult RetirarFiltroProdutosMaisVendidos()
        //{
        //    Session["ListProdMaisVendidos"] = null;
        //    return RedirectToAction("MontarTelaProdutosMaisVendidos");
        //}

        //[HttpPost]
        //public ActionResult MostrarTudoProdutosMaisVendidos()
        //{
        //    List<ITEM_PEDIDO_VENDA> ipv = pvApp.GetAllItensAdm().SelectMany(x => x.ITEM_PEDIDO_VENDA).ToList<ITEM_PEDIDO_VENDA>();
        //    List<PRODUTOS_MAIS_VENDIDOS> listaMaisVendidos = ipv.Where(x => x.PROD_CD_ID != null).GroupBy(x => x.PROD_CD_ID).Select(x => new PRODUTOS_MAIS_VENDIDOS { PROD_CD_ID = x.First().PROD_CD_ID.Value, PROD_NM_NOME = x.First().PRODUTO.PROD_NM_NOME, PROD_CD_CODIGO = x.First().PRODUTO.PROD_CD_CODIGO, PROD_NR_BARCODE = x.First().PRODUTO.PROD_NR_BARCODE, PRMV_QN_QUANTIDADE = x.Sum(v => (Int32)v.ITPE_QN_QUANTIDADE), PRMV_ULTIMA_VENDA = x.First().PEDIDO_VENDA.PEVE_DT_APROVACAO, PROD_DS_DESCRICAO = x.First().PRODUTO.PROD_DS_DESCRICAO, CAPR_CD_ID = x.First().PRODUTO.CAPR_CD_ID, SCPR_CD_ID = x.First().PRODUTO.SCPR_CD_ID }).ToList();
        //    SessionMocks.ListProdMaisVendidos = listaMaisVendidos;

        //    return RedirectToAction("MontarTelaProdutosMaisVendidos");
        //}

        //[HttpGet]
        //public ActionResult MontarTelaProdutosMaisVendidos()
        //{
        //    if (SessionMocks.ListProdMaisVendidos.Count == 0 || SessionMocks.ListProdMaisVendidos == null)
        //    {
        //        List<ITEM_PEDIDO_VENDA> ipv = pvApp.GetAllItens().SelectMany(x => x.ITEM_PEDIDO_VENDA).ToList<ITEM_PEDIDO_VENDA>();
        //        List<PRODUTOS_MAIS_VENDIDOS> listaMaisVendidos = ipv.Where(x => x.PROD_CD_ID != null).GroupBy(x => x.PROD_CD_ID).Select(x => new PRODUTOS_MAIS_VENDIDOS { PROD_CD_ID = x.First().PROD_CD_ID.Value, PROD_NM_NOME = x.First().PRODUTO.PROD_NM_NOME, PROD_CD_CODIGO = x.First().PRODUTO.PROD_CD_CODIGO, PROD_NR_BARCODE = x.First().PRODUTO.PROD_NR_BARCODE, PRMV_QN_QUANTIDADE = x.Sum(v => (Int32)v.ITPE_QN_QUANTIDADE), PRMV_ULTIMA_VENDA = x.First().PEDIDO_VENDA.PEVE_DT_APROVACAO, PROD_DS_DESCRICAO = x.First().PRODUTO.PROD_DS_DESCRICAO, CAPR_CD_ID = x.First().PRODUTO.CAPR_CD_ID, SCPR_CD_ID = x.First().PRODUTO.SCPR_CD_ID }).ToList();
        //        SessionMocks.ListProdMaisVendidos = listaMaisVendidos;
        //    }

        //    ViewBag.ListaMaisVendidos = SessionMocks.ListProdMaisVendidos;
        //    ViewBag.Categoria = cpApp.GetAllItens();
        //    ViewBag.SubCategoria = scpApp.GetAllItens();
        //    ViewBag.Produtos = prodApp.GetAllItens();

        //    return View();
        //}


        [HttpGet]
        public ActionResult EnviarEMailContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            FORNECEDOR item = fornApp.GetItemById(id);
            Session["Contato"] = item;
            ViewBag.Contato = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.FORN_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.FORN_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailContato(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailContato(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("EditarProduto", new { id = idNot });
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
        public ActionResult EnviarSMSContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            FORNECEDOR item = fornApp.GetItemById(id);
            Session["Contato"] = item;
            ViewBag.Contato = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.FORN_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.FORN_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSContato(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSContato(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("EditarProduto", new { id = idNot });
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailContato(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contato
            Int32 idAss = (Int32)Session["IdAssinante"];
            FORNECEDOR cont = (FORNECEDOR)Session["Contato"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.FORN_NM_NOME;

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO;
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Contato";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_DESTINO = cont.FORN_NM_EMAIL;
            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;

            // Envia mensagem
            try
            {
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
            }
            catch (Exception ex)
            {
                String erro = ex.Message;
            }
            return 0;
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSContato(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            FORNECEDOR cont = (FORNECEDOR)Session["Contato"];

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = vm.MENS_TX_SMS;

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(vm.MENS_TX_SMS);
            if (!String.IsNullOrEmpty(vm.LINK))
            {
                if (!vm.LINK.Contains("www."))
                {
                    vm.LINK = "www." + vm.LINK;
                }
                if (!vm.LINK.Contains("http://"))
                {
                    vm.LINK = "http://" + vm.LINK;
                }
                str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                texto += "  " + vm.LINK;
            }
            String body = str.ToString();
            String smsBody = body;
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(cont.FORN_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
            return 0;
        }


    }
}