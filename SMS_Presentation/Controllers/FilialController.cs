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

namespace ERP_CRM_Solution.Controllers
{
    public class FilialController : Controller
    {
        private readonly IFilialAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        FILIAL objetoForn = new FILIAL();
        FILIAL objetoFornAntes = new FILIAL();
        List<FILIAL> listaMasterForn = new List<FILIAL>();
        String extensao;

        public FilialController(IFilialAppService fornApps, ILogAppService logApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            confApp = confApps;
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["FilialVolta"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardAdministracao", "Usuario");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult PesquisaCNPJ(string cnpj)
        {
            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cnpj, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";

            try
            {
                WebResponse response = request.GetResponse();
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                var jObject = JObject.Parse(json);

                CNPJ pesquisaCNPJ = new CNPJ();
                pesquisaCNPJ.RAZAO = jObject["name"] == null ? String.Empty : jObject["name"].ToString();
                pesquisaCNPJ.NOME = jObject["alias"] == null ? jObject["name"].ToString() : jObject["alias"].ToString();
                pesquisaCNPJ.CEP = jObject["address"]["zip"].ToString();
                pesquisaCNPJ.ENDERECO = jObject["address"]["street"].ToString();
                pesquisaCNPJ.BAIRRO = jObject["address"]["neighborhood"].ToString();
                pesquisaCNPJ.CIDADE = jObject["address"]["city"].ToString();
                pesquisaCNPJ.UF = ((List<UF>)Session["UFs"]).Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                pesquisaCNPJ.INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                pesquisaCNPJ.TELEFONE = jObject["phone"].ToString();
                pesquisaCNPJ.EMAIL = jObject["email"].ToString();
                return Json(pesquisaCNPJ);
            }
            catch (WebException ex)
            {
                var hash = new Hashtable();
                hash.Add("status", "ERROR");

                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "BadRequest")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "CNPJ inválido");
                    return Json(hash);
                }
                if ((ex.Response as HttpWebResponse)?.StatusCode.ToString() == "NotFound")
                {
                    hash.Add("public", 1);
                    hash.Add("message", "O CNPJ consultado não está registrado na Receita Federal");
                    return Json(hash);
                }
                else
                {
                    hash.Add("public", 1);
                    hash.Add("message", ex.Message);
                    return Json(hash);
                }
            }
        }

        [HttpGet]
        public ActionResult MontarTelaFilial()
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

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaFilial"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaFilial"] = listaMasterForn;
            }
            ViewBag.Listas = (List<FILIAL>)Session["ListaFilial"];
            ViewBag.Title = "Filial";
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["IncluirFilial"] = 0;

            // Indicadores
            ViewBag.Filial = ((List<FILIAL>)Session["ListaFilial"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensFilial"] != null)
            {
                if ((Int32)Session["MensFilial"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFilial"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0058", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFilial"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0059", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFilial"] == 5)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFilial"] == 6)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new FILIAL();
            objetoForn.FILI_IN_ATIVO = 1;
            Session["MensFilial"] = 0;  
            Session["VoltaFilial"] = 1;
            Session["FilialVolta"] = 1;
            Session["FiltroFilial"] = null;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroFilial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaFilial"] = null;
            Session["FiltroFilial"] = null;
            return RedirectToAction("MontarTelaFilial");
        }

        public ActionResult MostrarTudoFilial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroFilial"] = null;
            Session["ListaFilial"] = listaMasterForn;
            return RedirectToAction("MontarTelaFilial");
        }

        [HttpGet]
        public ActionResult IncluirFilial()
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
                    Session["MensFilial"] = 2;
                    return RedirectToAction("MontarTelaFilial", "Filial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.TipoPessoa = new SelectList(((List<TIPO_PESSOA>)Session["TiposPessoas"]), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");

            // Prepara view
            FILIAL item = new FILIAL();
            FilialViewModel vm = Mapper.Map<FILIAL, FilialViewModel>(item);
            vm.FILI_IN_ATIVO = 1;
            vm.FILI_DT_CADASTRO = DateTime.Today;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirFilial(FilialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.TipoPessoa = new SelectList(((List<TIPO_PESSOA>)Session["TiposPessoas"]), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Checa logotipo
                    if (vm.FILI_AQ_LOGOTIPO == null)
                    {
                        vm.FILI_AQ_LOGOTIPO = "~/Imagens/Base/favicon_SystemBR.jpg";
                    }

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    FILIAL item = Mapper.Map<FilialViewModel, FILIAL>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensFilial"] = 3;
                        return RedirectToAction("MontarTelaFilial", "Filial");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Filial/" + item.FILI_CD_ID.ToString() + "/Logo/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVolta"] = item.FILI_CD_ID;
                    if (Session["FileQueueFilial"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueFilial"];

                        foreach (var file in fq)
                        {
                            UploadLogoQueueFilial(file);
                        }

                        Session["FileQueueFilial"] = null;
                    }

                    // Sucesso
                    listaMasterForn = new List<FILIAL>();
                    Session["ListaFilial"] = null;
                    return RedirectToAction("MontarTelaFilial");
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
        public ActionResult EditarFilial(Int32 id)
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
                    Session["MensFilial"] = 2;
                    return RedirectToAction("MontarTelaFilial", "Filial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            if ((Int32)Session["MensFilial"] == 4)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0059", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensFilial"] == 5)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }

            // Prepara listas
            ViewBag.TipoPessoa = new SelectList(((List<TIPO_PESSOA>)Session["TiposPessoas"]), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");

            // Prepara view
            FILIAL item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            FilialViewModel vm = Mapper.Map<FILIAL, FilialViewModel>(item);
            Session["IdFilial"] = id;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFilial(FilialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            ViewBag.TipoPessoa = new SelectList(((List<TIPO_PESSOA>)Session["TiposPessoas"]), "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    FILIAL item = Mapper.Map<FilialViewModel, FILIAL>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<FILIAL>();
                    Session["ListaFilial"] = null;
                    return RedirectToAction("MontarTelaFilial");
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
        public ActionResult ExcluirFilial(Int32 id)
        {
            // Valida acesso
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
                    Session["MensFilial"] = 2;
                    return RedirectToAction("MontarTelaFilial", "Filial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            FILIAL item = fornApp.GetItemById(id);
            objetoFornAntes = (FILIAL)Session["Filial"];
            item.FILI_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensFilial"] = 4;
                return RedirectToAction("MontarTelaFilial", "Filial");
            }
            listaMasterForn = new List<FILIAL>();
            Session["ListaFilial"] = null;
            Session["FiltroFilial"] = null;
            return RedirectToAction("MontarTelaFilial");
        }

        [HttpGet]
        public ActionResult ReativarFilial(Int32 id)
        {
            // Valida acesso
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
                    Session["MensFilial"] = 2;
                    return RedirectToAction("MontarTelaFilial", "Filial");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            FILIAL item = fornApp.GetItemById(id);
            objetoFornAntes = (FILIAL)Session["Filial"];
            item.FILI_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateReativar(item, usuario);
            listaMasterForn = new List<FILIAL>();
            Session["ListaFilial"] = null;
            Session["FiltroFilial"] = null;
            return RedirectToAction("MontarTelaFilial");
        }

        public ActionResult VoltarBaseFilial()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaFilial"] = null;
            return RedirectToAction("MontarTelaFilial");
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

            Session["FileQueueFilial"] = queue;
        }

        [HttpPost]
        public ActionResult UploadLogoQueueFilial(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdFilial"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensFilial"] = 5;
                return RedirectToAction("VoltarAnexoFilial");
            }
            FILIAL item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensFilial"] = 6;
                return RedirectToAction("VoltarAnexoFilial");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Filial/" + item.FILI_CD_ID.ToString() + "/Logo/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.FILI_AQ_LOGOTIPO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usu);
            listaMasterForn = new List<FILIAL>();
            Session["ListaFilial"] = null;
            return RedirectToAction("VoltarAnexoFilial");
        }

        [HttpPost]
        public ActionResult UploadLogoFilial(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdFilial"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensFilial"] = 5;
                return RedirectToAction("VoltarBaseFilial");
            }
            FILIAL item = fornApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensFilial"] = 6;
                return RedirectToAction("VoltarAnexoFilial");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Filial/" + item.FILI_CD_ID.ToString() + "/Logo/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.FILI_AQ_LOGOTIPO = "~" + caminho + fileName;
            objetoFornAntes = item;
            Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usu);
            listaMasterForn = new List<FILIAL>();
            Session["ListaFilial"] = null;
            return RedirectToAction("VoltarAnexoFilial");
        }

        [HttpPost]
        public JsonResult PesquisaCEP_Javascript(String cep, int tipoEnd)
        {
            // Chama servico ECT
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza
            var hash = new Hashtable();

            if (tipoEnd == 1)
            {
                hash.Add("FILI_NM_ENDERECO", end.Address + "/" + end.Complement);
                hash.Add("FILI_NM_BAIRRO", end.District);
                hash.Add("FILI_NM_CIDADE", end.City);
                hash.Add("FILI_SG_UF", end.Uf);
                hash.Add("UF_CD_ID", fornApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("FILI_NR_CEP", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public ActionResult VoltarAnexoFilial()
        {
            return RedirectToAction("EditarFilial", new { id = (Int32)Session["IdFilial"] });
        }

        [HttpGet]
        public ActionResult VerFilial(Int32 id)
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Incluir = (Int32)Session["IncluirForn"];

            FILIAL item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Filial"] = item;
            Session["IdVolta"] = id;
            Session["IdFilial"] = id;
            Session["VoltaCEP"] = 1;
            FilialViewModel vm = Mapper.Map<FILIAL, FilialViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EnviarEMailFilial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            FILIAL cont = fornApp.GetItemById(id);
            Session["Filial"] = cont;
            ViewBag.Filial = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.FILI_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.FILI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailFilial(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioEMailFilial(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseFilial");
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
        public Int32 ProcessaEnvioEMailFilial(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            FILIAL cont = (FILIAL)Session["Filial"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = String.Empty;
            if (cont.FILI_NM_CONTATOS != null)
            {
                cab = "Prezado Sr(a). <b>" + cont.FILI_NM_CONTATOS + "</b>";
            }
            else
            {
                cab = "Prezado Responsável pela Filial <b>" + cont.FILI_NM_NOME + "</b>";
            }

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
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
            mensagem.EMAIL_DESTINO = cont.FILI_NM_EMAIL;
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

        [HttpGet]
        public ActionResult EnviarSMSFilial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            FILIAL item = fornApp.GetItemById(id);
            Session["Filial"] = item;
            ViewBag.Filial = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.FILI_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.FILI_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSFilial(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSFilial(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseFilial");
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
        public Int32 ProcessaEnvioSMSFilial(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            FILIAL cont = (FILIAL)Session["Filial"];

            // Prepara cabeçalho
            String cab = String.Empty;
            if (cont.FILI_NM_CONTATOS != null)
            {
                cab = "Prezado Sr(a). <b>" + cont.FILI_NM_CONTATOS + "</b>";
            }
            else
            {
                cab = "Prezado Responsável pela Filial <b>" + cont.FILI_NM_NOME + "</b>";
            }

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = cab + vm.MENS_TX_SMS + rod;

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
                String listaDest = "55" + Regex.Replace(cont.FILI_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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