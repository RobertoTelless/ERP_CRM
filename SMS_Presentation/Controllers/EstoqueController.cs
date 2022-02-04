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
    public class EstoqueController : Controller
    {
        private readonly IProdutoAppService prodApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly IFilialAppService filApp;
        private readonly IMovimentoEstoqueProdutoAppService moepApp;
        private readonly IProdutoEstoqueFilialAppService pefApp;
        private readonly IFornecedorAppService fornApp;

        private String msg;
        private Exception exception;
        PRODUTO objetoProd = new PRODUTO();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        MOVIMENTO_ESTOQUE_PRODUTO objetoMOVP = new MOVIMENTO_ESTOQUE_PRODUTO();
        MOVIMENTO_ESTOQUE_PRODUTO objetoMOVPAntes = new MOVIMENTO_ESTOQUE_PRODUTO();
        List<MOVIMENTO_ESTOQUE_PRODUTO> listaMasterMOEP = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
        PRODUTO_ESTOQUE_FILIAL objetoProdFili = new PRODUTO_ESTOQUE_FILIAL();
        PRODUTO_ESTOQUE_FILIAL objetoProdFiliAntes = new PRODUTO_ESTOQUE_FILIAL();
        List<PRODUTO_ESTOQUE_FILIAL> listaMasterProdFili = new List<PRODUTO_ESTOQUE_FILIAL>();
        String extensao;

        public EstoqueController(IProdutoAppService prodApps, ICategoriaProdutoAppService cpApps, IFilialAppService filApps, IMovimentoEstoqueProdutoAppService moepApps, IProdutoEstoqueFilialAppService pefApps, IFornecedorAppService fornApps)
        {
            prodApp = prodApps;
            cpApp = cpApps;
            filApp = filApps;
            moepApp = moepApps;
            pefApp = pefApps;
            fornApp = fornApps;
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
        public JsonResult FiltrarSubCategoriaProduto(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaSubFiltrada = prodApp.GetAllSubs(idAss);

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaSubFiltrada = prodApp.GetAllSubs(idAss).Where(x => x.CAPR_CD_ID == id).ToList();
            }
            return Json(listaSubFiltrada.Select(x => new { value = x.SCPR_CD_ID, text = x.SCPR_NM_NOME }));
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

            return Json(listaFiltrada.Select(x => new { value = x.CAPR_CD_ID, text = x.CAPR_NM_NOME }));
        }

        [HttpGet]
        public ActionResult MontarTelaEstoqueProduto()
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
            if (Session["ListaProdEstoqueFilial"] == null)
            {
                listaMasterProdFili = pefApp.GetAllItens(idAss);
                Session["ListaProdEstoqueFilial"] = listaMasterProdFili;
            }
            if (usuario.FILI_CD_ID != null)
            {
                ViewBag.Listas = ((List<PRODUTO_ESTOQUE_FILIAL>)Session["ListaProdEstoqueFilial"]).Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).OrderBy(x => x.PRODUTO.PROD_NM_NOME).ToList<PRODUTO_ESTOQUE_FILIAL>();
            }
            else
            {
                ViewBag.Listas = ((List<PRODUTO_ESTOQUE_FILIAL>)Session["ListaProdEstoqueFilial"]).OrderBy(x => x.PRODUTO.PROD_NM_NOME).ToList<PRODUTO_ESTOQUE_FILIAL>();
            }

            ViewBag.Title = "Estoque";
            ViewBag.CatProd = new SelectList(prodApp.GetAllTipos(idAss).OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");

            // Indicadores
            ViewBag.Produtos = ((List<PRODUTO_ESTOQUE_FILIAL>)Session["ListaProdEstoqueFilial"]).Count;

            if (usuario.FILIAL != null)
            {
                ViewBag.FilialUsuario = usuario.FILIAL.FILI_NM_NOME;
            }

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.FilialUsuario = usuario.FILI_CD_ID;
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");

            // Mansagem
            if ((Int32)Session["MensEstoque"] == 1)
            {
                Session["MensEstoque"] = 0;
                ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
            }

            // Abre view
            Session["VoltaEstoque"] = 0;
            Session["MensEstoque"] = 0;
            objetoProd = new PRODUTO();
            if (Session["FiltroProduto"] != null)
            {
                objetoProd = (PRODUTO)Session["FiltroProduto"];
                Session["FiltroProduto"] = null;
            }
            listaMasterProdFili = null;
            Session["FiltroMvmtProd"] = false;
            Session["FiltroMov"] = false;
            //Session["ListaProdEstoqueFilial"] = null;
            return View(objetoProd);
        }

        public ActionResult RetirarFiltroProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaProdEstoqueFilial"] = null;
            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        public ActionResult RetirarFiltroMovimentacaoEstoqueProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMvmtProd"] = false;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = id });
        }

        public ActionResult RetirarFiltroMovimentacaoEstoqueProdutoNova(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMvmtProd"] = false;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = (Int32)Session["IdMovimento"] });
        }

        [HttpPost]
        public ActionResult FiltrarProduto(PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PRODUTO_ESTOQUE_FILIAL> listaObj = new List<PRODUTO_ESTOQUE_FILIAL>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                if (usuario.PERF_CD_ID != 1)
                {
                    item.FILI_CD_ID = usuario.FILI_CD_ID;
                }
                Int32 volta = prodApp.ExecuteFilterEstoque(item.FILI_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_NR_BARCODE, item.CAPR_CD_ID, item.PROD_IN_TIPO_PRODUTO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEstoque"] = 1;
                }

                // Sucesso
                listaMasterProdFili = listaObj;
                Session["ListaProdEstoqueFilial"] = listaObj;
                return RedirectToAction("MontarTelaEstoqueProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEstoqueProduto");
            }
        }

        public ActionResult VoltarBaseProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        public ActionResult GerarRelatorioEstoqueProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            var usuario = (USUARIO)Session["UserCredentials"];

            Int32 perfil = 0;
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                perfil = 1;
            }

            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ProdutoEstoqueLista" + "_" + data + ".pdf";
            List<PRODUTO_ESTOQUE_FILIAL> lista = pefApp.GetAllItens(usuario.ASSI_CD_ID);
            PRODUTO filtro = (PRODUTO)Session["FiltroProduto"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
            Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.GREEN);

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

            cell = new PdfPCell(new Paragraph("Estoque de Produtos - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 50f, 70f, 150f, 60f, 60f, 60f, 50f, 50f, 20f });
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


            cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Codigo de Barra", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Estoque", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Minima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Máxima", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Última Movimentação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Foto", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PRODUTO_ESTOQUE_FILIAL item in lista)
            {
                if (item.PRODUTO.PROD_IN_TIPO_PRODUTO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Produto", meuFont4))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Insumo", meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }

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
                    cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
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

                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NR_BARCODE, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PREF_QN_ESTOQUE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_QN_QUANTIDADE_MINIMA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_QN_QUANTIDADE_MAXIMA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_DT_ULTIMA_MOVIMENTACAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                if (item.PRODUTO.PROD_AQ_FOTO != null)
                {
                    Image foto = Image.GetInstance(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO));
                    cell = new PdfPCell(foto, true);
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph(" - ", meuFont))
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
                if (filtro.PROD_NR_BARCODE != null)
                {
                    parametros += "Código de Barras: " + filtro.PROD_NR_BARCODE;
                    ja = 1;
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
                if (filtro.CATEGORIA_PRODUTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Categoria: " + filtro.CATEGORIA_PRODUTO.CAPR_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Categoria: " + filtro.CATEGORIA_PRODUTO.CAPR_NM_NOME;
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
                if (perfil == 1)
                {
                    if (filtro.FILI_CD_ID != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Filial: " + filtro.FILI_CD_ID;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Filial: " + filtro.FILI_CD_ID;
                        }
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

            return RedirectToAction("MontarTelaEstoqueProduto");
        }

        [HttpGet]
        public ActionResult VerEstoqueProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            Session["VoltaEstoque"] = 1;
            return RedirectToAction("ConsultarProduto", "Produto", new { id = id });

        }

        [HttpGet]
        public ActionResult AcertoManualProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            var usu = (USUARIO)Session["UserCredentials"];
            PRODUTO_ESTOQUE_FILIAL item = pefApp.GetItemById(id);
            objetoProdFiliAntes = item;
            Session["ProdutoEstoqueFilial"] = item;
            Session["IdVolta"] = id;
            item.PREF_QN_QUANTIDADE_ALTERADA = item.PREF_QN_ESTOQUE;
            item.PREF_DS_JUSTIFICATIVA = String.Empty;
            return View(item);
        }

        [HttpPost]
        public ActionResult AcertoManualProduto(PRODUTO_ESTOQUE_FILIAL item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (item.PREF_QN_QUANTIDADE_ALTERADA == null)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM não pode ser nulo");
                return View(item);
            }
            if (item.PREF_QN_ESTOQUE == item.PREF_QN_QUANTIDADE_ALTERADA)
            {
                ModelState.AddModelError("", "Campo NOVA CONTAGEM com mesmo valor de ESTOQUE");
                return View(item);
            }
            if (item.PREF_DS_JUSTIFICATIVA == null)
            {
                ModelState.AddModelError("", "Campo JUSTIFICATIVA obrigatorio");
                return View(item);
            }
            if (item.FILIAL == null)
            {
                ModelState.AddModelError("", "Produto sem filial");
                return View(item);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = pefApp.ValidateEditEstoque(item, objetoProdFiliAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterProdFili = new List<PRODUTO_ESTOQUE_FILIAL>();
                    Session["ListaProdEstoqueFilial"] = null;
                    return RedirectToAction("MontarTelaEstoqueProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(objetoProdFiliAntes);
                }
            }
            else
            {
                return View(objetoProdFiliAntes);
            }
        }

        public ActionResult VerMovimentacaoEstoqueProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> filtroEntradaSaida = new List<SelectListItem>();
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.FiltroEntradaSaida = new SelectList(filtroEntradaSaida, "Value", "Text");

            USUARIO usu = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetItemById(id);
            PRODUTO item = prodApp.GetItemById(pef.PROD_CD_ID);
            if ((Boolean)Session["FiltroMvmtProd"])
            {
                Session["FiltroMvmtProd"] = false;
                item.MOVIMENTO_ESTOQUE_PRODUTO = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == (Int32)Session["EntradaSaida"]).ToList();
                objetoProdAntes = item;
            }
            item.MOVIMENTO_ESTOQUE_PRODUTO = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(x => x.FILI_CD_ID == pef.FILI_CD_ID).ToList();
            objetoProdAntes = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult VerMovimentacaoEstoqueProdutoNova(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> filtroEntradaSaida = new List<SelectListItem>();
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            filtroEntradaSaida.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.FiltroEntradaSaida = new SelectList(filtroEntradaSaida, "Value", "Text");

            USUARIO usu = (USUARIO)Session["UserCredentials"];

            // Prepara view
            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetItemById(id);
            PRODUTO item = prodApp.GetItemById(pef.PROD_CD_ID);
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(x => x.FILI_CD_ID == pef.FILI_CD_ID).ToList();
            
            // Filtro
            if ((Boolean)Session["FiltroMvmtProd"])
            {
                Session["FiltroMvmtProd"] = false;
                lista = lista.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == (Int32)Session["EntradaSaida"]).ToList();
                objetoProdAntes = item;
            }

            Session["ItemMovimento"] = pef;
            Session["IdMovimento"] = id;
            Session["ListaMovimento"] = lista;
            ViewBag.Lista = lista;
            objetoProdAntes = item;
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult VoltarAnexoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("VerEstoqueProduto", new { id = (Int32)Session["IdVolta"] });
        }

        public ActionResult RetirarFiltroInventario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroInventario"] = null;
            return RedirectToAction("MontarTelaInventario");
        }

        [HttpPost]
        public JsonResult IncluirMovimentacaoEstoque(String grid, String tipo, String justificativa)
        {
            var usuario = (USUARIO)Session["UserCredentials"];
            var listaAlterado = new List<Hashtable>();
            var jArray = JArray.Parse(grid);
            Int32 count = 0;

            if (tipo == "PROD")
            {
                foreach (var jObject in jArray)
                {
                    try
                    {
                        var pef = pefApp.GetItemById((Int32)jObject["idPef"]);

                        if ((Int32)jObject["qtde"] != pef.PREF_QN_ESTOQUE)
                        {
                            Int32 operacao = pef.PREF_QN_ESTOQUE < (Int32)jObject["qtde"] ? 1 : 2;
                            Int32 quant = 0;
                            if ((Int32)jObject["qtde"] > pef.PREF_QN_ESTOQUE)
                            {
                                quant = (Int32)jObject["qtde"] - (Int32)pef.PREF_QN_ESTOQUE;
                            }
                            else
                            {
                                quant = (Int32)pef.PREF_QN_ESTOQUE - (Int32)jObject["qtde"];
                            }

                            MOVIMENTO_ESTOQUE_PRODUTO movto = new MOVIMENTO_ESTOQUE_PRODUTO();
                            movto.MOEP_IN_CHAVE_ORIGEM = 2;
                            movto.MOEP_IN_ORIGEM = "Inventário";
                            movto.MOEP_IN_OPERACAO = operacao;
                            movto.MOEP_IN_TIPO_MOVIMENTO = 0;

                            movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            movto.FILI_CD_ID = pef.FILI_CD_ID;
                            movto.MOEP_DT_MOVIMENTO = DateTime.Today;
                            movto.MOEP_IN_ATIVO = 1;
                            movto.MOEP_QN_QUANTIDADE = (Int32)jObject["qtde"] - (Int32)pef.PREF_QN_ESTOQUE;
                            movto.PROD_CD_ID = pef.PROD_CD_ID;
                            movto.USUA_CD_ID = usuario.USUA_CD_ID;
                            movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            movto.MOEP_DS_JUSTIFICATIVA = justificativa;
                            movto.MOEP_QN_ANTES = pef.PREF_QN_ESTOQUE;
                            movto.MOEP_QN_ALTERADA = (Int32)jObject["qtde"] - pef.PREF_QN_ESTOQUE;
                            movto.MOEP_QN_DEPOIS = (Int32)jObject["qtde"];

                            PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
                            pef1.FILI_CD_ID = pef.FILI_CD_ID;
                            pef1.PREF_DS_JUSTIFICATIVA = justificativa;
                            pef1.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            pef1.PREF_IN_ATIVO = 1;
                            pef1.PREF_QN_ESTOQUE = (Int32)jObject["qtde"];
                            pef1.PROD_CD_ID = pef.PROD_CD_ID;
                            pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
                            pef1.PREF_CD_ID = pef.PREF_CD_ID;
                            pef1.PREF_QN_QUANTIDADE_ALTERADA = (Int32)jObject["qtde"] - pef.PREF_QN_ESTOQUE;

                            Int32 v = pefApp.ValidateEdit(pef1, pef, usuario);

                            var valAlterado = new Hashtable();
                            valAlterado.Add("id", movto.PROD_CD_ID);
                            valAlterado.Add("fili", movto.FILI_CD_ID);
                            valAlterado.Add("val", movto.MOEP_QN_ALTERADA);
                            listaAlterado.Add(valAlterado);

                            Int32 volta = moepApp.ValidateCreate(movto, usuario);

                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = ex.Message;
                    }
                }

                Session["ListaValEstoqueAlterado"] = listaAlterado;
                Session["listaProdEstoqueFilial"] = null;

                if (count == 0)
                {
                    return Json("Nenhum registro alterado");
                }
                else
                {
                    return Json("Foram alterados " + count + " produtos");
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacao(MOVIMENTO_ESTOQUE_PRODUTO prod, Int32 filtroES)
        {
            
            var retorno = new Hashtable();
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (prod != null)
            {
                try
                {
                    // Executa a operação
                    List<MOVIMENTO_ESTOQUE_PRODUTO> listaObjProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                    Int32 volta = moepApp.ExecuteFilter(prod.PRODUTO.CAPR_CD_ID, prod.PRODUTO.SCPR_CD_ID, prod.PRODUTO.PROD_NM_NOME, prod.PRODUTO.PROD_NR_BARCODE, prod.FILI_CD_ID, prod.MOEP_DT_MOVIMENTO, idAss, out listaObjProd);
                    Session["FiltroMovimentoProduto"] = prod;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEstoque"] = 1;
                        retorno.Add("error", "Nenhum registro localizado");
                        return Json(retorno);
                    }

                    // Filtra Entrada/Saída
                    listaObjProd = listaObjProd.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == filtroES).ToList();

                    // Sucesso
                    if (listaObjProd.Count > 0)
                    {
                        Session["ListaMovimentoProduto"] = listaObjProd;
                    }
                    else
                    {
                        retorno.Add("error", "Nenhum registro localizado");
                        return Json(retorno);
                    }

                    if (filtroES == 1)
                    {
                        Session["FiltroMovimentoEntrada"] = 1;
                        retorno.Add("url", "../Estoque/MontarTelaMovimentacaoEntrada");
                    }
                    else
                    {
                        Session["FiltroMovimentoSaida"] = 1;
                        retorno.Add("url", "../Estoque/MontarTelaMovimentacaoSaida");
                    }

                    Session["FiltroMovimentoProduto"] = prod;
                    return Json(retorno);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    retorno.Add("error", ex.Message);
                    return Json(retorno);
                }
            }
            return Json(retorno);

        }

        [HttpPost]
        public void LimparListas()
        {
            Session["ListaProduto"] = null;
            Session["ListaMovimentoProduto"] = null;
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoEstoqueProduto(ProdutoViewModel vm, Int32? EntradaSaida)
        {
            Session["FiltroMvmtProd"] = true;
            Session["EntradaSaida"] = vm.EntradaSaida;
            return RedirectToAction("VerMovimentacaoEstoqueProduto", new { id = vm.PROD_CD_ID });
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoEstoqueProdutoNova(ProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMvmtProd"] = true;
            Session["EntradaSaida"] = vm.EntradaSaida;
            return RedirectToAction("VerMovimentacaoEstoqueProdutoNova", new { id = (Int32)Session["IdMovimento"] });
        }

        public SelectList GetTipoEntrada()
        {
            List<SelectListItem> tipoEntrada = new List<SelectListItem>();
            tipoEntrada.Add(new SelectListItem() { Text = "Devolução", Value = "1" });
            tipoEntrada.Add(new SelectListItem() { Text = "Retorno de conserto", Value = "2" }); // Apenas Produto
            tipoEntrada.Add(new SelectListItem() { Text = "Reclassificação - À Definir", Value = "3" });
            tipoEntrada[2].Disabled = true;
            return new SelectList(tipoEntrada, "Value", "Text");
        }

        public SelectList GetTipoSaida()
        {
            List<SelectListItem> tipoSaida = new List<SelectListItem>();
            tipoSaida.Add(new SelectListItem() { Text = "Devolução do fornecedor", Value = "1" });
            tipoSaida.Add(new SelectListItem() { Text = "Remessa para conserto", Value = "2" }); //Apenas Produto
            tipoSaida.Add(new SelectListItem() { Text = "Baixa de insumos para produção - Em Desenvolvimento", Value = "3" });
            tipoSaida.Add(new SelectListItem() { Text = "Reclassificação - À Definir", Value = "4" });
            tipoSaida.Add(new SelectListItem() { Text = "Perda ou roubo", Value = "5" });
            tipoSaida.Add(new SelectListItem() { Text = "Descarte", Value = "6" });
            tipoSaida.Add(new SelectListItem() { Text = "Outras saídas", Value = "7" });
            tipoSaida[3].Disabled = true;
            tipoSaida[4].Disabled = true;
            return new SelectList(tipoSaida, "Value", "Text");
        }

        [HttpGet]
        public ActionResult MontarTelaMovimentacaoAvulsa()
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

            var vm = new MovimentacaoAvulsaGridViewModel();
            if (Session["FiltroMovEstoque"] != null)
            {
                vm = (MovimentacaoAvulsaGridViewModel)Session["FiltroMovEstoque"];
                Session["FiltroMovEstoque"] = null;
            }
            else
            {
                vm.ProdutoInsumo = 1;
                vm.MOVMT_DT_MOVIMENTO_INICIAL = DateTime.Now;
                FiltrarMovimentacaoAvulsa(vm);
            }

            ViewBag.Title = "Movimentações Avulsas";
            List<SelectListItem> prodIns = new List<SelectListItem>();
            prodIns.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            prodIns.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.ProdutoInsumo = new SelectList(prodIns, "Value", "Text");
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                lista.Add(new SelectListItem() { Text = "Transferência entre Filiais", Value = "4" });
            }
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            if (Session["FlagMovmtAvulsa"] != null)
            {
                if ((Int32)Session["FlagMovmtAvulsa"] == 1)
                {
                    ViewBag.TipoRegistro = 1;
                    ViewBag.ListaMovimento = ((List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoProduto"]).Where(x => x.PRODUTO != null && x.PRODUTO.PROD_IN_COMPOSTO == 0).Where(x => x.MOEP_IN_CHAVE_ORIGEM == 1 || x.MOEP_IN_CHAVE_ORIGEM == 5).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }
            }
            ViewBag.LstProd = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");

            if (Session["MensAvulsa"] != null)
            {
                if ((Int32)Session["MensAvulsa"] == 1)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensAvulsa"] = 0;
                }
            }            

            return View(vm);
        }

        public ActionResult RetirarFiltroMovimentacaoAvulsa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FlagMovmtAvulsa"] = null;
            Session["ListaMovimentoProduto"] = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        public ActionResult MostrarTudoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMovmtAvulsa"] = 1;
            Session["ListaMovimentoProduto"] = moepApp.GetAllItensAdm(idAss).Where(x => x.PRODUTO != null && x.PRODUTO.PROD_IN_COMPOSTO == 0).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpPost]
        public ActionResult FiltrarMovimentacaoAvulsa(MovimentacaoAvulsaGridViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["FiltroMovimentacaoAvulsa"] = vm;
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                Int32 volta = 0;
                var lstProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                var usuario = (USUARIO)Session["UserCredentials"];
                Session["FiltroMovEstoque"] = vm;

                if (vm.ProdutoInsumo == 1)
                {
                    Session["FlagMovmtAvulsa"] = 1;
                    volta = moepApp.ExecuteFilterAvulso(vm.MOVMT_IN_OPERACAO, vm.MOVMT_IN_TIPO_MOVIMENTO, vm.MOVMT_DT_MOVIMENTO_INICIAL, vm.MOVMT_DT_MOVIMENTO_FINAL, (usuario.FILI_CD_ID == null ? vm.FILI_CD_ID : usuario.FILI_CD_ID), vm.PROD_CD_ID, idAss, out lstProd);
                    Session["ListaMovimentoProduto"] = lstProd;
                }
                if (volta == 1)
                {
                    Session["MensAvulsa"] = 1;
                }

                return RedirectToAction("MontarTelaMovimentacaoAvulsa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaMovimentacaoAvulsa");
            }
        }

        //PRODUTO
        [HttpGet]
        public ActionResult ExcluirMovimentoProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Verifica se tem usuario logado
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Monta movimento
            var item = moepApp.GetById(id);
            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
            mov.ASSI_CD_ID = item.ASSI_CD_ID;
            mov.FILI_CD_ID = item.FILI_CD_ID;
            mov.MOEP_CD_ID = item.MOEP_CD_ID;
            mov.MOEP_DS_JUSTIFICATIVA = item.MOEP_DS_JUSTIFICATIVA;
            mov.MOEP_DT_MOVIMENTO = item.MOEP_DT_MOVIMENTO;
            mov.MOEP_IN_ATIVO = 0;
            mov.MOEP_IN_CHAVE_ORIGEM = item.MOEP_IN_CHAVE_ORIGEM;
            mov.MOEP_IN_ORIGEM = item.MOEP_IN_ORIGEM;
            mov.MOEP_IN_TIPO_MOVIMENTO = item.MOEP_IN_TIPO_MOVIMENTO;
            mov.MOEP_QN_ANTES = item.MOEP_QN_ANTES;
            mov.MOEP_QN_DEPOIS = item.MOEP_QN_DEPOIS;
            mov.MOEP_QN_QUANTIDADE = item.MOEP_QN_QUANTIDADE;
            mov.PROD_CD_ID = item.PROD_CD_ID;
            mov.USUA_CD_ID = item.USUA_CD_ID;
            mov.MOEP_IN_OPERACAO = item.MOEP_IN_OPERACAO;

            //Monta Estoque filial
            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial(item.PROD_CD_ID, (Int32)item.FILI_CD_ID);
            if (pef != null)
            {
                PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
                pef1.FILI_CD_ID = pef.FILI_CD_ID;
                pef1.PREF_CD_ID = pef.PREF_CD_ID;
                pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
                pef1.PREF_DT_ULTIMO_MOVIMENTO = pef.PREF_DT_ULTIMO_MOVIMENTO;
                pef1.PREF_IN_ATIVO = pef.PREF_IN_ATIVO;
                pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
                pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE;
                pef1.PREF_QN_QUANTIDADE_ALTERADA = pef.PREF_QN_QUANTIDADE_ALTERADA;
                pef1.PROD_CD_ID = pef.PROD_CD_ID;

                //Efetua Operações
                if (mov.MOEP_IN_TIPO_MOVIMENTO == 1 || mov.MOEP_IN_TIPO_MOVIMENTO == 4)
                {
                    pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE - (Int32)mov.MOEP_QN_QUANTIDADE;
                    Int32 ve = pefApp.ValidateEdit(pef1, pef, usuario);
                }
                else
                {
                    pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE + (Int32)mov.MOEP_QN_QUANTIDADE;
                    Int32 vs = pefApp.ValidateEdit(pef1, pef, usuario);
                }
            }
            Int32 volta = moepApp.ValidateDelete(mov, usuario);
            Session["FlagMovmtAvulsa"] = null;
            Session["ListaMovimentoProduto"] = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult ReativarMovimentoProduto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Verifica se tem usuario logado
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Monta movimento
            var item = moepApp.GetById(id);
            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
            mov.ASSI_CD_ID = item.ASSI_CD_ID;
            mov.FILI_CD_ID = item.FILI_CD_ID;
            mov.MOEP_CD_ID = item.MOEP_CD_ID;
            mov.MOEP_DS_JUSTIFICATIVA = item.MOEP_DS_JUSTIFICATIVA;
            mov.MOEP_DT_MOVIMENTO = item.MOEP_DT_MOVIMENTO;
            mov.MOEP_IN_ATIVO = 1;
            mov.MOEP_IN_CHAVE_ORIGEM = item.MOEP_IN_CHAVE_ORIGEM;
            mov.MOEP_IN_ORIGEM = item.MOEP_IN_ORIGEM;
            mov.MOEP_IN_TIPO_MOVIMENTO = item.MOEP_IN_TIPO_MOVIMENTO;
            mov.MOEP_QN_ANTES = item.MOEP_QN_ANTES;
            mov.MOEP_QN_DEPOIS = item.MOEP_QN_DEPOIS;
            mov.MOEP_QN_QUANTIDADE = item.MOEP_QN_QUANTIDADE;
            mov.PROD_CD_ID = item.PROD_CD_ID;
            mov.USUA_CD_ID = item.USUA_CD_ID;
            mov.MOEP_IN_OPERACAO = item.MOEP_IN_OPERACAO;

            //Monta Estoque filial
            var pef = pefApp.GetByProdFilial(item.PROD_CD_ID, (Int32)item.FILI_CD_ID);
            PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
            pef1.FILI_CD_ID = pef.FILI_CD_ID;
            pef1.PREF_CD_ID = pef.PREF_CD_ID;
            pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
            pef1.PREF_DT_ULTIMO_MOVIMENTO = pef.PREF_DT_ULTIMO_MOVIMENTO;
            pef1.PREF_IN_ATIVO = pef.PREF_IN_ATIVO;
            pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
            pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE;
            pef1.PREF_QN_QUANTIDADE_ALTERADA = pef.PREF_QN_QUANTIDADE_ALTERADA;
            pef1.PROD_CD_ID = pef.PROD_CD_ID;

            //Executa Operações
            if (mov.MOEP_IN_TIPO_MOVIMENTO == 1 || mov.MOEP_IN_TIPO_MOVIMENTO == 4)
            {
                pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE + (Int32)mov.MOEP_QN_QUANTIDADE;
                Int32 ve = pefApp.ValidateEdit(pef1, pef, usuario);
            }
            else
            {
                pef1.PREF_QN_ESTOQUE = pef1.PREF_QN_ESTOQUE - (Int32)mov.MOEP_QN_QUANTIDADE;
                Int32 vs = pefApp.ValidateEdit(pef1, pef, usuario);
            }
            item.MOEP_IN_ATIVO = 0;
            Int32 volta = moepApp.ValidateReativar(mov, usuario);
            Session["FlagMovmtAvulsa"] = null;
            Session["ListaMovimentoProduto"] = null;
            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult IncluirMovimentacaoAvulsa()
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

            ViewBag.Title = "Movimentações Avulsas - Lançamento";
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                lista.Add(new SelectListItem() { Text = "Zeramento de Estoque", Value = "3" });
                lista.Add(new SelectListItem() { Text = "Transferência entre Filiais", Value = "4" });
            }
            lista.Add(new SelectListItem() { Text = "Compra Expressa", Value = "5" });
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.ListaProdutos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");

            MovimentacaoAvulsaViewModel vm = new MovimentacaoAvulsaViewModel();
            vm.MOVMT_DT_MOVIMENTO = DateTime.Now;
            if (Session["MovAvulsaVM"] != null)
            {
                vm = (MovimentacaoAvulsaViewModel)Session["MovAvulsaVM"];
                Session["MovAvulsaVM"] = null;
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirMovimentacaoAvulsa(MovimentacaoAvulsaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            var usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Title = "Lançamentos - Produtos";
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            lista.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            if (usuario.PERF_CD_ID == 1 || usuario.PERF_CD_ID == 2)
            {
                lista.Add(new SelectListItem() { Text = "Zeramento de Estoque", Value = "3" });
                lista.Add(new SelectListItem() { Text = "Transferência entre Filiais", Value = "4" });
            }
            ViewBag.ListaMovimentoProd = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoProduto"];
            ViewBag.Lista = new SelectList(lista, "Value", "Text");
            ViewBag.Entradas = GetTipoEntrada();
            ViewBag.Saidas = GetTipoSaida();
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.ListaProdutos = new SelectList(prodApp.GetAllItens(idAss).Where(x => x.PROD_IN_COMPOSTO == 0).OrderBy(x => x.PROD_NM_NOME).ToList<PRODUTO>(), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Fornecedores = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");

            var listaMvmtProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            if (ModelState.IsValid)
            {
                if (vm.REGISTROS == null)
                {
                    ModelState.AddModelError("", "Nenhum registro adicionado para inclusão");
                    return View(vm);
                }

                if (usuario.PERF_CD_ID != 1 && usuario.PERF_CD_ID != 2)
                {
                    vm.FILI_CD_ID = usuario.FILI_CD_ID.Value;
                }

                    for (var i = 0; i < vm.REGISTROS.Count(); i++)
                    {
                        try
                        {
                            Int32 qnAntes = 0;
                            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial(vm.REGISTROS[i], vm.FILI_CD_ID);

                            if (pef == null)
                            {
                                pef = new PRODUTO_ESTOQUE_FILIAL();
                                pef.FILI_CD_ID = vm.FILI_CD_ID;
                                pef.PROD_CD_ID = vm.REGISTROS[i];
                                pef.PREF_QN_QUANTIDADE_ALTERADA = 0;
                                pef.PREF_QN_ESTOQUE = 0;
                                pef.PREF_IN_ATIVO = 1;
                                pef.PREF_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                Int32 v = pefApp.ValidateCreate(pef, usuario);
                                pef = pefApp.GetByProdFilial(vm.REGISTROS[i], vm.FILI_CD_ID);
                            }

                            PRODUTO_ESTOQUE_FILIAL pefAntes = pef;
                            qnAntes = pef.PREF_QN_ESTOQUE == null ? 0 : (Int32)pef.PREF_QN_ESTOQUE;

                            if (vm.MOVMT_IN_OPERACAO == 1)
                            {
                                if (vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA == null)
                                {
                                    ModelState.AddModelError("", "Campo TIPO DE MOVIMENTAÇÃO obrigatorio");
                                    return View(vm);
                                }

                                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE + pef.PREF_QN_QUANTIDADE_ALTERADA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 2)
                            {
                                if (vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA == null)
                                {
                                    ModelState.AddModelError("", "Campo TIPO DE MOVIMENTAÇÃO obrigatorio");
                                    return View(vm);
                                }
                                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE - pef.PREF_QN_QUANTIDADE_ALTERADA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 3)
                            {
                                pef.PREF_QN_QUANTIDADE_ALTERADA = pef.PREF_QN_ESTOQUE;
                                pef.PREF_QN_ESTOQUE = 0;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 4)
                            {
                                PRODUTO_ESTOQUE_FILIAL pefDestino = pefApp.GetByProdFilial(vm.REGISTROS[i], (Int32)vm.FILI_DESTINO_CD_ID);

                                if (pefDestino == null)
                                {
                                    pefDestino = new PRODUTO_ESTOQUE_FILIAL();
                                    pefDestino.FILI_CD_ID = vm.FILI_CD_ID;
                                    pefDestino.PROD_CD_ID = vm.REGISTROS[i];
                                    pefDestino.PREF_QN_ESTOQUE = 0;
                                    pefDestino.PREF_QN_QUANTIDADE_ALTERADA = 0;
                                    pefDestino.PREF_IN_ATIVO = 1;
                                    pefDestino.PREF_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                                    pefDestino.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                    Int32 v = pefApp.ValidateCreate(pefDestino, usuario);
                                    pefDestino = pefApp.CheckExist(pefDestino,idAss);
                                }
                                PRODUTO_ESTOQUE_FILIAL pefDestinoAntes = pefDestino;
                                qnAntes = pefDestino.PREF_QN_ESTOQUE == null ? 0 : (Int32)pefDestino.PREF_QN_ESTOQUE;
                                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pef.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE - pef.PREF_QN_QUANTIDADE_ALTERADA;
                                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                pefDestino.PREF_QN_QUANTIDADE_ALTERADA = vm.QUANTIDADE[i];
                                pefDestino.PREF_QN_ESTOQUE = pefDestino.PREF_QN_ESTOQUE + pefDestino.PREF_QN_QUANTIDADE_ALTERADA;
                                pefDestino.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

                                Int32 voltaPefDestino = pefApp.ValidateEdit(pefDestino, pefDestinoAntes, usuario);
                            }

                            mov.MOEP_QN_ANTES = qnAntes;
                            mov.MOEP_QN_DEPOIS = pef.PREF_QN_ESTOQUE;
                            mov.MOEP_IN_OPERACAO = vm.MOVMT_IN_OPERACAO;
                            if (vm.MOVMT_IN_OPERACAO == 1)
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = 1;
                                mov.MOEP_IN_ORIGEM = GetTipoEntrada().Where(x => x.Value == vm.MOVMT_IN_TIPO_MOVIMENTO_ENTRADA.ToString()).Select(x => x.Text).First();
                            }
                            else if (vm.MOVMT_IN_OPERACAO == 2)
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = 2;
                                mov.MOEP_IN_ORIGEM = GetTipoSaida().Where(x => x.Value == vm.MOVMT_IN_TIPO_MOVIMENTO_SAIDA.ToString()).Select(x => x.Text).First();
                            }
                            else if(vm.MOVMT_IN_OPERACAO == 3)
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = 2;
                                mov.MOEP_IN_ORIGEM = "Zeramento de Estoque";
                            }
                            else if(vm.MOVMT_IN_OPERACAO == 4)
                            {
                                mov.MOEP_IN_TIPO_MOVIMENTO = 2;
                                mov.MOEP_IN_ORIGEM = "Tranferência entre filiais";
                            }
                            mov.MOEP_IN_CHAVE_ORIGEM = 1;
                            mov.PROD_CD_ID = vm.REGISTROS[i];
                            mov.MOEP_QN_QUANTIDADE = vm.QUANTIDADE[i];
                            mov.MOEP_DT_MOVIMENTO = vm.MOVMT_DT_MOVIMENTO;
                            mov.FILI_CD_ID = vm.FILI_CD_ID;
                            mov.MOEP_DS_JUSTIFICATIVA = vm.MOVMT_DS_JUSTIFICATIVA;
                            mov.MOEP_IN_ATIVO = 1;
                            mov.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            mov.USUA_CD_ID = usuario.USUA_CD_ID;
                            mov.MOEP_QN_ANTES = 0;
                            mov.MOEP_QN_ALTERADA = vm.QUANTIDADE[i];
                            mov.MOEP_QN_DEPOIS = vm.QUANTIDADE[i];
                            listaMvmtProd.Add(mov);

                            // Se Transferencia...
                            if (vm.MOVMT_IN_OPERACAO == 4)
                            {

                            }

                            Int32 voltaPef = pefApp.ValidateEdit(pef, pefAntes, usuario);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.Message;
                            return View();
                        }
                    }

                    Int32 volta = moepApp.ValidateCreateLista(listaMvmtProd);

                    foreach (var lp in listaMvmtProd)
                    {
                        lp.PRODUTO = prodApp.GetById(lp.PROD_CD_ID);
                        lp.FILIAL = filApp.GetById(lp.FILI_CD_ID);
                    }

                    ViewBag.flagProdIns = 1;
                    ViewBag.ListaMovimento = listaMvmtProd;
                    Session["ListaMovimentoProduto"] = listaMvmtProd;
                    Session["ListaProdEstoqueFilial"] = null;
                    if (vm.btnVolta == null)
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaMovimentacaoAvulsa");
                    }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        //public ActionResult IncluirCompraExpressa(MovimentacaoAvulsaViewModel vm)
        //{
            

        //    Session["MovAvulsaVM"] = vm;
        //    USUARIO usuario = new USUARIO();

        //    if (vm.ProdutoInsumoEx == 1)
        //    {
        //        if (vm.PROD_CD_ID == null)
        //        {
        //            Session["MensCompraEx"] = 1;
        //            return RedirectToAction("IncluirMovimentacaoAvulsa");
        //        }
        //        if (vm.PROD_CD_ID == null)
        //        {
        //            Session["MensCompraEx"] = 2;
        //            return RedirectToAction("IncluirMovimentacaoAvulsa");
        //        }

        //        PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial((Int32)vm.PROD_CD_ID, (Int32)vm.FILI_CD_ID_EX);
        //        PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();

        //        Int32 qnAntes = 0;

        //        if (pef == null)
        //        {
        //            pef = new PRODUTO_ESTOQUE_FILIAL();
        //            pef.FILI_CD_ID = vm.FILI_CD_ID_EX;
        //            pef.PROD_CD_ID = (Int32)vm.PROD_CD_ID;
        //            pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
        //            pef.PREF_QN_ESTOQUE = vm.QTDE_PROD;
        //            pef.PREF_IN_ATIVO = 1;
        //            pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;

        //            Int32 v = pefApp.ValidateCreate(pef, usuario);
        //        } 
        //        else
        //        {
        //            pef1.PREF_CD_ID = pef.PREF_CD_ID;
        //            pef1.FILI_CD_ID = pef.FILI_CD_ID;
        //            pef1.PROD_CD_ID = pef.PROD_CD_ID;
        //            pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE + vm.QTDE_PROD;
        //            pef1.PREF_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
        //            pef1.PREF_IN_ATIVO = 1;
        //            pef1.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
        //            pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
        //            pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;

        //            Int32 v = pefApp.ValidateEdit(pef1, pef, usuario);

        //            qnAntes = (Int32)pef.PREF_QN_ESTOQUE;
        //        }

        //        MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
        //        mov.ASSI_CD_ID = SessionMocks.IdAssinante;
        //        mov.FILI_CD_ID = pef.FILI_CD_ID;
        //        mov.MOEP_DT_MOVIMENTO = DateTime.Now;
        //        mov.MOEP_IN_ATIVO = 1;
        //        mov.MOEP_IN_CHAVE_ORIGEM = 5;
        //        mov.MOEP_IN_OPERACAO = 1;
        //        mov.MOEP_IN_ORIGEM = "Compra Expressa";
        //        mov.MOEP_IN_TIPO_MOVIMENTO = 0;
        //        mov.MOEP_QN_ALTERADA = vm.QTDE_PROD;
        //        mov.MOEP_QN_ANTES = qnAntes;
        //        mov.MOEP_QN_DEPOIS = vm.QTDE_PROD;
        //        mov.MOEP_QN_QUANTIDADE = (Int32)vm.QTDE_PROD;
        //        mov.PROD_CD_ID = (Int32)vm.PROD_CD_ID;
        //        mov.USUA_CD_ID = usuario.USUA_CD_ID;

        //        Int32 volta = moepApp.ValidateCreate(mov, usuario);
        //    }
        //    else if (vm.ProdutoInsumoEx == 2)
        //    {
        //        if (vm.MAPR_CD_ID == null)
        //        {
        //            Session["MensCompraEx"] = 3;
        //            return RedirectToAction("IncluirMovimentacaoAvulsa");
        //        }
        //        if (vm.QTDE_MAPR == null)
        //        {
        //            Session["MensCompraEx"] = 4;
        //            return RedirectToAction("IncluirMovimentacaoAvulsa");
        //        }

        //        MATERIA_PRIMA_ESTOQUE_FILIAL mef = mefApp.GetByInsFilial((Int32)vm.MAPR_CD_ID, (Int32)vm.FILI_CD_ID_EX);
        //        MATERIA_PRIMA_ESTOQUE_FILIAL mef1 = new MATERIA_PRIMA_ESTOQUE_FILIAL();

        //        Int32 qnAntes = 0;

        //        if (mef == null)
        //        {
        //            mef = new MATERIA_PRIMA_ESTOQUE_FILIAL();
        //            mef.FILI_CD_ID = vm.FILI_CD_ID_EX;
        //            mef.MAPR_CD_ID = (Int32)vm.PROD_CD_ID;
        //            mef.MPFE_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
        //            mef.MPFE_QN_ESTOQUE = (Int32)vm.QTDE_MAPR;
        //            mef.MPFE_IN_ATIVO = 1;
        //            mef.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;

        //            Int32 v = mefApp.ValidateCreate(mef, usuario);
        //        }
        //        else
        //        {
        //            mef1.MPFE_CD_ID = mef.MPFE_CD_ID;
        //            mef1.FILI_CD_ID = mef.FILI_CD_ID;
        //            mef1.MAPR_CD_ID = mef.MAPR_CD_ID;
        //            mef1.MPFE_QN_ESTOQUE = mef.MPFE_QN_ESTOQUE + (Int32)vm.QTDE_MAPR;
        //            mef1.MPFE_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
        //            mef1.MPFE_IN_ATIVO = 1;
        //            mef1.MPFE_DT_ULTIMO_MOVIMENTO = DateTime.Now;
        //            mef1.MPFE_DS_JUSTIFICATIVA = mef.MPFE_DS_JUSTIFICATIVA;

        //            Int32 v = mefApp.ValidateEdit(mef1, mef, usuario);

        //            qnAntes = (Int32)mef.MPFE_QN_ESTOQUE;
        //        }

        //        MOVIMENTO_ESTOQUE_MATERIA_PRIMA mov = new MOVIMENTO_ESTOQUE_MATERIA_PRIMA();
        //        mov.ASSI_CD_ID = SessionMocks.IdAssinante;
        //        mov.FILI_CD_ID = mef.FILI_CD_ID;
        //        mov.MOEM_DT_MOVIMENTO = DateTime.Now;
        //        mov.MOEM_IN_ATIVO = 1;
        //        mov.MOEM_IN_CHAVE_ORIGEM = 5;
        //        mov.MOEM_IN_OPERACAO = 1;
        //        mov.MOEM_NM_ORIGEM = "Compra Expressa";
        //        mov.MOEM_IN_TIPO_MOVIMENTO = 0;
        //        mov.MOEM_QN_ALTERADA = vm.QTDE_PROD;
        //        mov.MOEM_QN_ANTES = qnAntes;
        //        mov.MOEM_QN_DEPOIS = vm.QTDE_PROD;
        //        mov.MOEM_QN_QUANTIDADE = (Int32)vm.QTDE_PROD;
        //        mov.MAPR_CD_ID = (Int32)vm.PROD_CD_ID;
        //        mov.USUA_CD_ID = usuario.USUA_CD_ID;

        //        Int32 volta = moemApp.ValidateCreate(mov, usuario);
        //    }

        //    CONTA_PAGAR cp = new CONTA_PAGAR();
        //    cp.FORN_CD_ID = vm.FORN_CD_ID;
        //    cp.USUA_CD_ID = usuario.USUA_CD_ID;
        //    cp.CAPA_DT_LANCAMENTO = DateTime.Now;
        //    cp.CAPA_DT_COMPETENCIA = DateTime.Now;
        //    cp.CAPA_DT_VENCIMENTO = DateTime.Now.AddDays(30);

        //    cp.ASSI_CD_ID = SessionMocks.IdAssinante;
        //    cp.CAPA_IN_ATIVO = 1;
        //    cp.CAPA_IN_LIQUIDADA = 0;
        //    cp.CAPA_IN_PAGA_PARCIAL = 0;
        //    cp.CAPA_IN_PARCELADA = 0;
        //    cp.CAPA_IN_PARCELAS = 0;
        //    cp.CAPA_VL_SALDO = 0;
        //    cp.CAPA_IN_CHEQUE = 0;

        //    SessionMocks.contaPagar = cp;
        //    SessionMocks.voltaCompra = 2;

        //    return RedirectToAction("IncluirCP", "ContaPagar");
        //}

        public ActionResult GerarRelatorioMovimentacaoAvulsaFiltro()
        {
            
            return RedirectToAction("GerarRelatorioMovimentacaoAvulsa", new { id = 1 });
        }

        [HttpGet]
        public ActionResult GerarRelatorioMovimentacaoAvulsa(Int32 id)
        {
            
            MovimentacaoAvulsaGridViewModel vm = (MovimentacaoAvulsaGridViewModel)Session["FiltroMovimentacaoAvulsa"];

            Int32 filtroProdIns = vm.ProdutoInsumo == null ? 1 : (Int32)vm.ProdutoInsumo;

            var listaMvmtProd = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            var filtroProd = new MOVIMENTO_ESTOQUE_PRODUTO();
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "MvmtAvulsaLista" + "_" + data + ".pdf";
            String titulo = "Movimentações Avulsas - Listagem";
            String operacao = String.Empty;
            String tipomvmt = String.Empty;
            String operacaoFiltro = String.Empty;
            String tipomvmtFiltro = String.Empty;
            if (Session["ListaMovimentoProduto"] != null)
            {
                if (id == 1)
                {
                    listaMvmtProd = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoProduto"];
                }
                else
                {
                    return RedirectToAction("MontarTelaMovimentacaoAvulsa");
                }
            }
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

            String info = String.Empty;
            if (filtroProdIns == 1)
            {
                info = "Produtos selecionados pelos parametros de filtro abaixo";
            }
            else if (filtroProdIns == 2)
            {
                info = "Insumos selecionados pelos parametros de filtro abaixo";
            }

            cell = new PdfPCell(new Paragraph(info, meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Operação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tipo de Movimentação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data do Movimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Produto", meuFont))
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
            cell = new PdfPCell(new Paragraph("Quantidade Antes", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Alterada", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Quantidade Depois", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (var prod in listaMvmtProd)
            {
                if (prod.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    operacao = "Entrada";
                }
                else if (prod.MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    operacao = "Saída";
                }
                else if (prod.MOEP_IN_TIPO_MOVIMENTO == 3)
                {
                    operacao = "Zeramento de Estoque";
                }
                else if (prod.MOEP_IN_TIPO_MOVIMENTO == 4)
                {
                    operacao = "Transferência entre Filiais";
                }
                cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(operacao) ? "-" : operacao, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);

                if (prod.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    if (prod.MOEP_IN_CHAVE_ORIGEM == 1)
                    {
                        tipomvmt = "Devolução";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 2)
                    {
                        tipomvmt = "Retorno de conserto";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 2)
                    {
                        tipomvmt = "Reclassificação";
                    }
                    else
                    {
                        tipomvmt = "-";
                    }
                }
                else if (prod.MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    if (prod.MOEP_IN_CHAVE_ORIGEM == 1)
                    {
                        tipomvmt = "Devolução do fornecedor";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 2)
                    {
                        tipomvmt = "Remessa para conserto";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 3)
                    {
                        tipomvmt = "Baixa de insumos para produção";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 4)
                    {
                        tipomvmt = "Reclassificação";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 5)
                    {
                        tipomvmt = "Perda ou roubo";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 6)
                    {
                        tipomvmt = "Descarte";
                    }
                    else if (prod.MOEP_IN_CHAVE_ORIGEM == 7)
                    {
                        tipomvmt = "Outras saídas";
                    }
                    else
                    {
                        tipomvmt = "-";
                    }
                }
                else
                {
                    tipomvmt = "Tranferência entre filiais";
                }
                cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(tipomvmt) ? "-" : tipomvmt, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(prod.MOEP_DT_MOVIMENTO == null ? "-" : prod.MOEP_DT_MOVIMENTO.ToString("dd/MM/yyyy"), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(String.IsNullOrEmpty(prod.PRODUTO.PROD_NM_NOME) ? "-" : prod.PRODUTO.PROD_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (prod.FILIAL != null)
                {
                    cell = new PdfPCell(new Paragraph(prod.FILIAL.FILI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Sem Filial", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(prod.MOEP_QN_ANTES.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(prod.MOEP_QN_QUANTIDADE.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(prod.MOEP_QN_DEPOIS.ToString(), meuFont))
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

            if (vm.MOVMT_IN_TIPO_MOVIMENTO != null)
            {
                parametros += "Operacao: " + operacaoFiltro;
                ja = 1;
            }
            if (vm.MOVMT_DT_MOVIMENTO_INICIAL != DateTime.MinValue && vm.MOVMT_DT_MOVIMENTO_INICIAL != null)
            {
                if (ja == 0)
                {
                    parametros += "Data Inicial: " + vm.MOVMT_DT_MOVIMENTO_INICIAL.Value.ToString("dd/MM/yyyy");
                    ja = 1;
                }
                else
                {
                    parametros += "e Data Inicial: " + vm.MOVMT_DT_MOVIMENTO_INICIAL.Value.ToString("dd/MM/yyyy");
                }
            }
            if (vm.MOVMT_DT_MOVIMENTO_FINAL != DateTime.MinValue && vm.MOVMT_DT_MOVIMENTO_FINAL != null)
            {
                if (ja == 0)
                {
                    parametros += "Data Final: " + vm.MOVMT_DT_MOVIMENTO_FINAL.Value.ToString("dd/MM/yyyy");
                    ja = 1;
                }
                else
                {
                    parametros += "e Data Final: " + vm.MOVMT_DT_MOVIMENTO_FINAL.Value.ToString("dd/MM/yyyy");
                }
            }
            if (vm.FILI_CD_ID != null)
            {
                var nmefilial = filApp.GetById(vm.FILI_CD_ID).FILI_NM_NOME;
                if (ja == 0)
                {
                    parametros += "Filial: " + nmefilial;
                    ja = 1;
                }
                else
                {
                    parametros += "e Filial: " + nmefilial;
                }
            }
            else
            {
                if (ja == 0)
                {
                    parametros += "Filial: Sem Filial";
                    ja = 1;
                }
                else
                {
                    parametros += "e Filial: Sem Filial";
                }
            }
            if (vm.MOVMT_IN_CHAVE_ORIGEM != null)
            {
                if (ja == 0)
                {
                    parametros += "Tipo de Movimento: " + tipomvmtFiltro;
                    ja = 1;
                }
                else
                {
                    parametros += "e Tipo de Movimento: " + tipomvmtFiltro;
                }
            }
            if (ja == 0)
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

            return RedirectToAction("MontarTelaMovimentacaoAvulsa");
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardEstoque()
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
                    Session["MensEstoque"] = 2;
                    return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
                }
                if ((Int32)Session["PermMens"] == 0)
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

            // Recupera listas e contagem
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaTotal = moepApp.GetAllItens(idAss);
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
            {
                listaTotal = listaTotal.Where(p => p.FILI_CD_ID == usuario.FILI_CD_ID).ToList();
            }            
            
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaTotalEntrada = listaTotal.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 1).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaTotalSaida= listaTotal.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 2).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMes = listaTotal.Where(p => p.MOEP_DT_MOVIMENTO.Month == DateTime.Today.Date.Month & p.MOEP_DT_MOVIMENTO.Year == DateTime.Today.Date.Year).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesEntrada = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 1).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesSaida = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 2).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesCompra = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 5).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesZeramento = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 3).ToList();
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaMesTransf = listaMes.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 4).ToList();

            // Produtos
            List<PRODUTO> prodTotal = prodApp.GetAllItens(idAss);
            Int32 prodTotalNum = prodTotal.Count;
            Int32 prods = prodTotal.Where(p => p.PROD_IN_TIPO_PRODUTO == 1).ToList().Count;
            Int32 ins = prodTotal.Where(p => p.PROD_IN_TIPO_PRODUTO == 2).ToList().Count;
            ViewBag.ProdTotal = prodTotalNum;
            ViewBag.Prods = prods;
            ViewBag.Ins = ins;

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
                ViewBag.PontoPedido = pontoPedido.Count;
                ViewBag.EstoqueZerado = estoqueZerado.Count;
                ViewBag.EstoqueNegativo = estoqueNegativo.Count;
            }
            else
            {
                pontoPedido = pontoPedido.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList<PRODUTO_ESTOQUE_FILIAL>();
                estoqueZerado = estoqueZerado.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList<PRODUTO_ESTOQUE_FILIAL>();
                estoqueNegativo = estoqueNegativo.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList<PRODUTO_ESTOQUE_FILIAL>();
                ViewBag.PontoPedido = pontoPedido.Count;
                ViewBag.EstoqueZerado = estoqueZerado.Count;
                ViewBag.EstoqueNegativo = estoqueNegativo.Count;
            }

            // Prepara View 
            ViewBag.Total = listaTotal;
            ViewBag.TotalSum = listaTotal.Count;
            ViewBag.TotalEntrada = listaTotalEntrada;
            ViewBag.TotalEntradaSum = listaTotalEntrada.Count;
            ViewBag.TotalSaida = listaTotalSaida;
            ViewBag.TotalSaidaSum = listaTotalSaida.Count;
            
            ViewBag.Mes = listaMes;
            ViewBag.MesSum = listaMes.Count;
            ViewBag.MesEntrada = listaMesEntrada;
            ViewBag.MesEntradaSum = (listaMesEntrada.Count) + (listaMesCompra.Count);
            ViewBag.MesSaida = listaMesSaida;
            ViewBag.MesSaidaSum = (listaMesSaida.Count) + (listaMesZeramento.Count);

            // Resumo Mes 
            List<DateTime> datas = listaMes.Select(p => p.MOEP_DT_MOVIMENTO.Date).Distinct().ToList();
            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            foreach (DateTime item in datas)
            {
                Int32 conta = listaMes.Where(p => p.MOEP_DT_MOVIMENTO.Date == item).Count();
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor = conta;
                lista.Add(mod1);
            }
            ViewBag.ListaMov = lista;
            ViewBag.ListaMovSum = lista.Count;
            Session["ListaDatas"] = datas;
            Session["ListaMovResumo"] = lista;
            Session["Entradas"] = listaMesEntrada.Count;
            Session["Saidas"] = (listaMesSaida.Count);
            Session["Zeramento"] = (listaMesZeramento.Count);
            Session["Transf"] = (listaMesTransf.Count);
            Session["Compra"] = (listaMesCompra.Count);
            Session["ListaMes"] = listaMes;

            // Resumo Tipo  
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.Data = "Entradas";
            mod.Valor = listaMesEntrada.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Saídas";
            mod.Valor = listaMesSaida.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Compra Expressa";
            mod.Valor = listaMesCompra.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Zeramento de Estoque";
            mod.Valor = listaMesZeramento.Count;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Transferência";
            mod.Valor = listaMesTransf.Count;
            lista1.Add(mod);
            ViewBag.ListaSituacao = lista1;
            Session["ListaSituacao"] = lista1;
            Session["VoltaDash"] = 3;
            Session["VoltaProdutoDash"] = 5;
            Session["VoltaFTDash"] = 5;
            return View(vm);
        }

        public JsonResult GetDadosGraficoMovimentosTipo()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["Entradas"];
            Int32 q2 = (Int32)Session["Saidas"];
            Int32 q3 = (Int32)Session["Zeramento"];
            Int32 q4 = (Int32)Session["Transf"];
            Int32 q5 = (Int32)Session["Compra"];

            desc.Add("Entradas");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Saídas");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("Zeramento de Estoque");
            quant.Add(q3);
            cor.Add("#FF7F00");
            desc.Add("Transferências");
            quant.Add(q4);
            cor.Add("#D63131");
            desc.Add("Compras Expressas");
            quant.Add(q5);
            cor.Add("#27A1C6");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoMovimentos()
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaCP1 = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMes"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatas"];
            List<MOVIMENTO_ESTOQUE_PRODUTO> listaDia = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MOEP_DT_MOVIMENTO.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardCompraDummy()
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
                    Session["MensEstoque"] = 2;
                    return RedirectToAction("MontarTelaEstoqueProduto", "Estoque");
                }
                if ((Int32)Session["PermMens"] == 0)
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

        [HttpPost]
        public ActionResult IncluirCompraExpressa(MovimentacaoAvulsaViewModel vm)
        {          
            Session["MovAvulsaVM"] = vm;
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (vm.PROD_CD_ID == null)
            {
                Session["MensCompraEx"] = 1;
                return RedirectToAction("IncluirMovimentacaoAvulsa");
            }
            if (vm.PROD_CD_ID == null)
            {
                Session["MensCompraEx"] = 2;
                return RedirectToAction("IncluirMovimentacaoAvulsa");
            }

            PRODUTO_ESTOQUE_FILIAL pef = pefApp.GetByProdFilial((Int32)vm.PROD_CD_ID, (Int32)vm.FILI_CD_ID_EX);
            PRODUTO_ESTOQUE_FILIAL pef1 = new PRODUTO_ESTOQUE_FILIAL();
            Int32 qnAntes = 0;
            if (pef == null)
            {
                pef = new PRODUTO_ESTOQUE_FILIAL();
                pef.FILI_CD_ID = vm.FILI_CD_ID_EX;
                pef.PROD_CD_ID = (Int32)vm.PROD_CD_ID;
                pef.PREF_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
                pef.PREF_QN_ESTOQUE = vm.QTDE_PROD;
                pef.PREF_IN_ATIVO = 1;
                pef.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                Int32 v = pefApp.ValidateCreate(pef, usuario);
            } 
            else
            {
                pef1.PREF_CD_ID = pef.PREF_CD_ID;
                pef1.FILI_CD_ID = pef.FILI_CD_ID;
                pef1.PROD_CD_ID = pef.PROD_CD_ID;
                pef1.PREF_QN_ESTOQUE = pef.PREF_QN_ESTOQUE + vm.QTDE_PROD;
                pef1.PREF_QN_QUANTIDADE_ALTERADA = vm.QTDE_PROD;
                pef1.PREF_IN_ATIVO = 1;
                pef1.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Now;
                pef1.PREF_DS_JUSTIFICATIVA = pef.PREF_DS_JUSTIFICATIVA;
                pef1.PREF_NR_MARKUP = pef.PREF_NR_MARKUP;
                Int32 v = pefApp.ValidateEdit(pef1, pef, usuario);
                qnAntes = (Int32)pef.PREF_QN_ESTOQUE;
            }

            MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
            mov.ASSI_CD_ID = idAss;
            mov.FILI_CD_ID = pef.FILI_CD_ID;
            mov.MOEP_DT_MOVIMENTO = DateTime.Now;
            mov.MOEP_IN_ATIVO = 1;
            mov.MOEP_IN_CHAVE_ORIGEM = 5;
            mov.MOEP_IN_OPERACAO = 1;
            mov.MOEP_IN_ORIGEM = "Compra Expressa";
            mov.MOEP_IN_TIPO_MOVIMENTO = 5;
            mov.MOEP_QN_ALTERADA = vm.QTDE_PROD;
            mov.MOEP_QN_ANTES = qnAntes;
            mov.MOEP_QN_DEPOIS = vm.QTDE_PROD;
            mov.MOEP_QN_QUANTIDADE = (Int32)vm.QTDE_PROD;
            mov.PROD_CD_ID = (Int32)vm.PROD_CD_ID;
            mov.USUA_CD_ID = usuario.USUA_CD_ID;
            Int32 volta = moepApp.ValidateCreate(mov, usuario);

            CONTA_PAGAR cp = new CONTA_PAGAR();
            cp.FORN_CD_ID = vm.FORN_CD_ID;
            cp.USUA_CD_ID = usuario.USUA_CD_ID;
            cp.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
            cp.CAPA_DT_COMPETENCIA = DateTime.Today.Date;
            cp.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            cp.ASSI_CD_ID = idAss;
            cp.CAPA_IN_ATIVO = 1;
            cp.CAPA_IN_LIQUIDADA = 0;
            cp.CAPA_IN_PAGA_PARCIAL = 0;
            cp.CAPA_IN_PARCELADA = 0;
            cp.CAPA_IN_PARCELAS = 0;
            cp.CAPA_VL_SALDO = 0;
            cp.CAPA_IN_CHEQUE = 0;
            cp.CAPA_VL_VALOR = vm.QTDE_PROD * vm.Preco;
            cp.CAPA_DS_DESCRICAO = "Compra expressa em " + DateTime.Today.Date.ToShortDateString();
            cp.CAPA_VL_DESCONTO = 0;
            cp.CAPA_VL_JUROS = 0;
            cp.CAPA_VL_TAXAS = 0;
            cp.USUA_CD_ID = usuario.USUA_CD_ID;
            cp.CAPA_IN_CHEQUE = 0;

            Session["ContaPagar"] = cp;
            Session["VoltaCpmpra"] = 2;
            Session["ListaProdEstoqueFilial"] = null;
            return RedirectToAction("IncluirCPExpressa", "ContaPagar");
        }

        [HttpGet]
        public ActionResult MontarTelaInventario()
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
            
            InventarioViewModel vm = new InventarioViewModel();
            if (Session["FiltroInventario"] != null)
            {
                vm = (InventarioViewModel)Session["FiltroInventario"];
            }

            if (Session["ListaProdEstoqueFilial"] == null)
            {
                if (Session["FiltroInventario"] != null)
                {
                    vm = (InventarioViewModel)Session["FiltroInventario"];
                    Session["FiltroInventario"] = null;
                    return FiltrarInventario(vm);
                }
                else
                {
                    if (vm.PRODUTO != null)
                    {
                        vm.PRODUTO = new PRODUTO();
                    }
                    else
                    {
                        vm.PRODUTO = new PRODUTO();
                    }
                    return FiltrarInventario(vm);
                }
            }

            ViewBag.Title = "Inventário";
            ViewBag.ListaProd = Session["ListaProdEstoqueFilial"] == null ? null : (List<PRODUTO_ESTOQUE_FILIAL>)Session["ListaProdEstoqueFilial"];
            ViewBag.Diferenca = Session["ListaValEstoqueAlterado"];
            List<SelectListItem> filtroPM = new List<SelectListItem>();
            filtroPM.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            filtroPM.Add(new SelectListItem() { Text = "Insumo", Value = "2" });
            ViewBag.CatProd = new SelectList(cpApp.GetAllItens(idAss).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.SubCatProd = new SelectList(prodApp.GetAllSubs(idAss).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            if (Session["ListaProdEstoqueFilial"] != null)
            {
                ViewBag.IsProdIns = 1;
            }
            ViewBag.CdFiliUsuario = usuario.FILI_CD_ID == null ? 1 : usuario.FILI_CD_ID;
            ViewBag.FilialUsuario = usuario.FILIAL == null ? "Sem Filial" : usuario.FILIAL.FILI_NM_NOME;
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss).OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME", "Selecionar");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensEstoque"] != null)
            {
                if ((Int32)Session["MensEstoque"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensEstoque"] = 0;
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult FiltrarInventario(InventarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FiltroInventario"] = vm;

            PRODUTO prod = vm.PRODUTO;
            if (vm.TIPO == 1 || vm.TIPO == null)
            {
                prod.FILI_CD_ID = vm.FILI_CD_ID_P;
                try
                {
                    // Executa a operação
                    List<PRODUTO_ESTOQUE_FILIAL> listaObj = new List<PRODUTO_ESTOQUE_FILIAL>();
                    if (usuario.FILI_CD_ID != null)
                    {
                        prod.FILI_CD_ID = usuario.FILI_CD_ID;
                    }
                    Int32 volta = prodApp.ExecuteFilterEstoque(prod.FILI_CD_ID, prod.PROD_NM_NOME, prod.PROD_NM_MARCA, prod.PROD_CD_CODIGO, prod.PROD_NR_BARCODE, prod.CAPR_CD_ID, prod.PROD_IN_TIPO_PRODUTO, idAss, out listaObj);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensEstoque"] = 1;
                    }

                    // Sucesso
                    listaMasterProdFili = listaObj;
                    Session["ListaProdEstoqueFilial"] = listaObj;

                    return RedirectToAction("MontarTelaInventario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return RedirectToAction("MontarTelaInventario");
                }
            }
            else
            {
                return RedirectToAction("MontarTelaInventario");
            }
        }
    }
}