using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SMS_Presentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_CRM_Solution.ViewModels;
using System.IO;
using Correios.Net;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EntitiesServices.Attributes;
using OfficeOpenXml.Table;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;

namespace ERP_CRM_Solution.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly ITipoPessoaAppService tpApp;
        private readonly IFilialAppService filApp;
        //private readonly IContaReceberAppService crApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IClienteCnpjAppService ccnpjApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        CLIENTE objeto = new CLIENTE();
        CLIENTE objetoAntes = new CLIENTE();
        List<CLIENTE> listaMaster = new List<CLIENTE>();
        List<CONTA_RECEBER> listaMasterRec = new List<CONTA_RECEBER>();
        String extensao;

        public ClienteController(IClienteAppService baseApps, ILogAppService logApps, ITipoPessoaAppService tpApps, IFilialAppService filApps, IUsuarioAppService usuApps, IClienteCnpjAppService ccnpjApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            tpApp = tpApps;
            filApp = filApps;
            usuApp = usuApps;
            ccnpjApp = ccnpjApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
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

        public ActionResult EnviarSmsCliente(Int32 id, String mensagem)
        {
            try
            {
                CLIENTE clie = baseApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Verifica existencia prévia
                if (clie == null)
                {
                    Session["MensSMSClie"] = 1;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Criticas
                if (clie.CLIE_NR_CELULAR == null)
                {
                    Session["MensSMSClie"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }

                // Monta token
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = mensagem;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(texto);
                String body = str.ToString();
                String smsBody = body;

                // inicia processo
                String resposta = String.Empty;
                try
                {
                    String listaDest = "55" + Regex.Replace(clie.CLIE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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
                    String erro = ex.Message;
                }

                Session["MensSMSClie"] = 200;
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                Session["MensSMSClie"] = 3;
                Session["MensSMSClieErro"] = ex.Message;
                return RedirectToAction("MontarTelaCliente");
            }
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CLIENTE> clientes = baseApp.GetAllItens(idAss);
            Session["Clientes"] = clientes;

            if (nome != null)
            {
                List<CLIENTE> lstCliente = clientes.Where(x => x.CLIE_NM_NOME != null && x.CLIE_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.CLIE_NM_RAZAO != null).ToList<CLIENTE>();
                    lstCliente = lstCliente.Where(x => x.CLIE_NM_RAZAO.ToLower().Contains(nome.ToLower())).ToList<CLIENTE>();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.CLIE_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.CLIE_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.CLIE_NM_NOME + " (" + item.CLIE_NM_RAZAO + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        public ActionResult DashboardAdministracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            listaMaster = new List<CLIENTE>();
            Session["Cliente"] = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        public void FlagContinua()
        {
            Session["VoltaCliente"] = 3;
        }

        [HttpPost]
        public JsonResult GetValorGrafico(Int32 id, Int32? meses)
        {
            if (meses == null)
            {
                meses = 3;
            }

            var clie = baseApp.GetById(id);

            Int32 m1 = clie.PEDIDO_VENDA.Where(x => x.PEVE_DT_APROVACAO >= DateTime.Now.AddDays(DateTime.Now.Day * -1)).SelectMany(x => x.ITEM_PEDIDO_VENDA).Sum(x => x.ITPE_QN_QUANTIDADE);
            Int32 m2 = clie.PEDIDO_VENDA.Where(x => x.PEVE_DT_APROVACAO >= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-1) && x.PEVE_DT_APROVACAO <= DateTime.Now.AddDays(DateTime.Now.Day * -1)).SelectMany(x => x.ITEM_PEDIDO_VENDA).Sum(x => x.ITPE_QN_QUANTIDADE);
            Int32 m3 = clie.PEDIDO_VENDA.Where(x => x.PEVE_DT_APROVACAO >= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-2) && x.PEVE_DT_APROVACAO <= DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-1)).SelectMany(x => x.ITEM_PEDIDO_VENDA).Sum(x => x.ITPE_QN_QUANTIDADE);

            var hash = new Hashtable();
            hash.Add("m1", m1);
            hash.Add("m2", m2);
            hash.Add("m3", m3);

            return Json(hash);
        }

        [HttpPost]
        public JsonResult PesquisaCNPJ(string cnpj)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lstQs = new List<CLIENTE_QUADRO_SOCIETARIO>();

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

                if (jObject["membership"].Count() == 0)
                {
                    CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                    qs.CLIENTE = new CLIENTE();
                    qs.CLIENTE.CLIE_NM_RAZAO = jObject["name"] == null ? String.Empty : jObject["name"].ToString();
                    qs.CLIENTE.CLIE_NM_NOME = jObject["alias"] == null ? jObject["name"].ToString() : jObject["alias"].ToString();
                    qs.CLIENTE.CLIE_NR_CEP = jObject["address"]["zip"].ToString();
                    qs.CLIENTE.CLIE_NM_ENDERECO = jObject["address"]["street"].ToString();
                    qs.CLIENTE.CLIE_NR_NUMERO = jObject["address"]["number"].ToString();
                    qs.CLIENTE.CLIE_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                    qs.CLIENTE.CLIE_NM_CIDADE = jObject["address"]["city"].ToString();
                    qs.CLIENTE.UF_CD_ID = ((List<UF>)Session["UFs"]).Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                    qs.CLIENTE.CLIE_NR_INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                    qs.CLIENTE.CLIE_NR_TELEFONE = jObject["phone"].ToString();
                    qs.CLIENTE.CLIE_NR_TELEFONE_ADICIONAL = jObject["phone_alt"].ToString();
                    qs.CLIENTE.CLIE_NM_EMAIL = jObject["email"].ToString();
                    qs.CLIENTE.CLIE_NM_SITUACAO = jObject["registration"]["status"].ToString();
                    qs.CLQS_IN_ATIVO = 0;

                    lstQs.Add(qs);
                }
                else
                {
                    foreach (var s in jObject["membership"])
                    {
                        CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                        qs.CLIENTE = new CLIENTE();
                        qs.CLIENTE.CLIE_NM_RAZAO = jObject["name"].ToString() == "" ? String.Empty : jObject["name"].ToString();
                        qs.CLIENTE.CLIE_NM_NOME = jObject["alias"].ToString() == "" ? jObject["name"].ToString() : jObject["alias"].ToString();
                        qs.CLIENTE.CLIE_NR_CEP = jObject["address"]["zip"].ToString();
                        qs.CLIENTE.CLIE_NM_ENDERECO = jObject["address"]["street"].ToString();
                        qs.CLIENTE.CLIE_NR_NUMERO = jObject["address"]["number"].ToString();
                        qs.CLIENTE.CLIE_NM_BAIRRO = jObject["address"]["neighborhood"].ToString();
                        qs.CLIENTE.CLIE_NM_CIDADE = jObject["address"]["city"].ToString();
                        qs.CLIENTE.UF_CD_ID = ((List<UF>)Session["UFs"]).Where(x => x.UF_SG_SIGLA == jObject["address"]["state"].ToString()).Select(x => x.UF_CD_ID).FirstOrDefault();
                        qs.CLIENTE.CLIE_NR_INSCRICAO_ESTADUAL = jObject["sintegra"]["home_state_registration"].ToString();
                        qs.CLIENTE.CLIE_NR_TELEFONE = jObject["phone"].ToString();
                        qs.CLIENTE.CLIE_NR_TELEFONE_ADICIONAL = jObject["phone_alt"].ToString();
                        qs.CLIENTE.CLIE_NM_EMAIL = jObject["email"].ToString();
                        qs.CLIENTE.CLIE_NM_SITUACAO = jObject["registration"]["status"].ToString();
                        qs.CLQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                        qs.CLQS_NM_NOME = s["name"].ToString();

                        // CNPJá não retorna esses valores
                        qs.CLQS_NM_PAIS_ORIGEM = String.Empty;
                        qs.CLQS_NM_REPRESENTANTE_LEGAL = String.Empty;
                        qs.CLQS_NM_QUALIFICACAO_REP_LEGAL = String.Empty;

                        lstQs.Add(qs);
                    }
                }

                return Json(lstQs);
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

        private List<CLIENTE_QUADRO_SOCIETARIO> PesquisaCNPJ(CLIENTE cliente)
        {
            List<CLIENTE_QUADRO_SOCIETARIO> lstQs = new List<CLIENTE_QUADRO_SOCIETARIO>();

            var url = "https://api.cnpja.com.br/companies/" + Regex.Replace(cliente.CLIE_NR_CNPJ, "[^0-9]", "");
            String json = String.Empty;

            WebRequest request = WebRequest.Create(url);
            request.Headers["Authorization"] = "df3c411d-bb44-41eb-9304-871c45d72978-cd751b62-ff3d-4421-a9d2-b97e01ca6d2b";

            WebResponse response = request.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
            {
                json = reader.ReadToEnd();
            }

            var jObject = JObject.Parse(json);

            foreach (var s in jObject["membership"])
            {
                CLIENTE_QUADRO_SOCIETARIO qs = new CLIENTE_QUADRO_SOCIETARIO();

                qs.CLQS_NM_QUALIFICACAO = s["role"]["description"].ToString();
                qs.CLQS_NM_NOME = s["name"].ToString();
                qs.CLIE_CD_ID = cliente.CLIE_CD_ID;

                // CNPJá não retorna esses valores
                qs.CLQS_NM_PAIS_ORIGEM = String.Empty;
                qs.CLQS_NM_REPRESENTANTE_LEGAL = String.Empty;
                qs.CLQS_NM_QUALIFICACAO_REP_LEGAL = String.Empty;

                lstQs.Add(qs);
            }

            return lstQs;
        }

        [HttpGet]
        public ActionResult MontarTelaCliente()
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
            if ((List<CLIENTE>)Session["ListaCliente"] == null || ((List<CLIENTE>)Session["ListaCliente"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCliente"] = listaMaster;
            }
            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.UF = new SelectList((List<UF>)Session["UFs"], "UF_CD_ID", "UF_NM_NOME");
            Session["Cliente"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");
            Session["IncluirCliente"] = 0;

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaCliente"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            //ViewBag.Atrasos = crApp.GetItensAtrasoCliente().Select(x => x.CLIE_CD_ID).Distinct().ToList().Count;
            ViewBag.Atrasos = 0;
            ViewBag.Inativos = baseApp.GetAllItensAdm(idAss).Where(p => p.CLIE_IN_ATIVO == 0).ToList().Count;
            ViewBag.SemPedidos = baseApp.GetAllItens(idAss).Where(p => p.PEDIDO_VENDA.Count == 0 || p.PEDIDO_VENDA == null).ToList().Count;
            //ViewBag.ContasAtrasos = SessionMocks.listaCR;
            ViewBag.ContasAtrasos = 0;
            ViewBag.CodigoCliente = Session["IdCliente"];

            if (Session["MensCliente"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCliente"] == 2)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 3)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0029", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 4)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0030", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0080", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensCliente"] = 0;
            Session["VoltaCliente"] = 1;
            objeto = new CLIENTE();
            if (Session["FiltroCliente"] != null)
            {
                objeto = (CLIENTE)Session["FiltroCliente"];
            }
            objeto.CLIE_IN_ATIVO = 1;
            return View(objeto);
        }

        public ActionResult RetirarFiltroCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCliente"] = null;
            Session["FiltroCliente"] = null;
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult MostrarTudoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["FiltroCliente"] = null;
            Session["ListaCliente"] = listaMaster;
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            return RedirectToAction("MontarTelaCliente");
        }

        [HttpPost]
        public ActionResult FiltrarCliente(CLIENTE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CLIENTE> listaObj = new List<CLIENTE>();
                Session["FiltroCliente"] = item;
                Int32 volta = baseApp.ExecuteFilter(item.CLIE_CD_ID, item.CACL_CD_ID, item.CLIE_NM_RAZAO, item.CLIE_NM_NOME, item.CLIE_NR_CPF, item.CLIE_NR_CNPJ, item.CLIE_NM_EMAIL, item.CLIE_NM_CIDADE, item.UF_CD_ID, item.CLIE_IN_ATIVO, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("MontarTelaCliente");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaCliente"]  = listaObj;
                if ((Int32)Session["VoltaCliente"] == 2)
                {
                    return RedirectToAction("VerCardsCliente");
                }
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCliente");
            }
        }

        public ActionResult VoltarBaseCliente()
        {
            if ((Int32)Session["VoltaCliente"] == 2)
            {
                return RedirectToAction("VerCardsCliente");
            }
            if ((Int32)Session["VoltaCliente"] == 3)
            {
                return RedirectToAction("VerClientesAtraso");
            }
            if ((Int32)Session["VoltaCliente"] == 4)
            {
                return RedirectToAction("VerClientesSemPedidos");
            }
            if ((Int32)Session["VoltaCliente"] == 5)
            {
                return RedirectToAction("VerClientesInativos");
            }
            if ((Int32)Session["VoltaCliente"] == 6)
            {
                return RedirectToAction("IncluirAtendimento", "Atendimento");
            }
            if ((Int32)Session["VoltaCliente"] == 7)
            {
                return RedirectToAction("IncluirOrdemServico", "OrdemServico");
            }
            if ((Int32)Session["VoltaCRM"] == 11)
            {
                return RedirectToAction("VoltarBaseCRM", "CRM");
            }
            return RedirectToAction("MontarTelaCliente");
        }

       [HttpGet]
        public ActionResult IncluirCliente()
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var filiais = filApp.GetAllItens(idAss);

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumClientes"] <= num)
            {
                Session["MensCliente"] = 50;
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }

            // Prepara listas
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");
            if (filiais.Count != 0)
            {
                ViewBag.Filiais = new SelectList(filiais, "FILI_CD_ID", "FILI_NM_NOME");
            }
            ViewBag.TiposPessoa = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.TiposCont = new SelectList(baseApp.GetAllContribuinte(idAss), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(idAss), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(baseApp.GetAllSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Usuarios = new SelectList((List<USUARIO>)Session["Usuarios"], "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");

            // Prepara view
            Session["ClienteNovo"] = 0;
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CLIE_DT_CADASTRO = DateTime.Today;
            vm.CLIE_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.TIPE_CD_ID = 0;
            vm.FILI_CD_ID = usuario.FILI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCliente(ClienteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TiposPessoa = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.TiposCont = new SelectList(baseApp.GetAllContribuinte(idAss), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(idAss), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(baseApp.GetAllSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Usuarios = new SelectList((List<USUARIO>)Session["Usuarios"], "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCliente"] = 3;
                        return RedirectToAction("MontarTelaCliente", "Cliente");
                    }

                    // Carrega foto e processa alteracao
                    item.CLIE_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                    volta = baseApp.ValidateEdit(item, item, usuario);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirCliente"] = 1;
                    Session["ClienteNovo"] = item.CLIE_CD_ID;
                    Session["Clientes"] = baseApp.GetAllItens(idAss);

                    if (item.TIPE_CD_ID == 2)
                    {
                        var lstQs = PesquisaCNPJ(item);

                        foreach (var qs in lstQs)
                        {
                            Int32 voltaQs = ccnpjApp.ValidateCreate(qs, usuario);
                        }
                    }

                    Session["IdVolta"] = item.CLIE_CD_ID;
                    Session["IdCliente"] = item.CLIE_CD_ID;
                    if (Session["FileQueueCliente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCliente"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueCliente(file);
                            }
                            else
                            {
                                UploadFotoQueueCliente(file);
                            }
                        }

                        Session["FileQueueCliente"] = null;
                    }

                    if ((Int32)Session["VoltaCliente"] == 6)
                    {
                        return RedirectToAction("IncluirAtendimento", "Atendimento");
                    }
                    if ((Int32)Session["VoltaCliente"] == 7)
                    {
                        return RedirectToAction("IncluirOrdemServico", "OrdemServico");
                    }
                    //if (SessionMocks.ClienteToCr)
                    //{
                    //    SessionMocks.ClienteToCr = false;
                    //    return RedirectToAction("IncluirCR", "ContaReceber");
                    //}
                    if ((Int32)Session["VoltaCliente"] == 3)
                    {
                        Session["VoltaCliente"] = 0;
                        return RedirectToAction("IncluirCliente", "Cliente");
                    }
                    return RedirectToAction("MontarTelaCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                vm.TIPE_CD_ID = 0;
                vm.SEXO_CD_ID = 0;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCliente(Int32 id)
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.TiposPessoa = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.TiposCont = new SelectList(baseApp.GetAllContribuinte(idAss), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(idAss), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(baseApp.GetAllSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Usuarios = new SelectList((List<USUARIO>)Session["Usuarios"], "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");

            CLIENTE item = baseApp.GetItemById(id);
            ViewBag.QuadroSoci = ccnpjApp.GetByCliente(item);

            if (Session["MensCliente"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCliente"] == 5)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCliente"] == 6)
                {
                    ModelState.AddModelError("", SMS_Mensagens.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
            }

            // Indicadores
            //ViewBag.Vendas = item.PEDIDO_VENDA.Count;
            //ViewBag.Servicos = 0;
            //ViewBag.Atendimentos = item.ATENDIMENTO.Count;
            ViewBag.Vendas = 12;
            ViewBag.Servicos = 0;
            ViewBag.Atendimentos = 8;
            ViewBag.Incluir = (Int32)Session["IncluirCliente"];

            Session["VoltaCliente"] = 1;
            objetoAntes = item;
            Session["Cliente"] = item;
            Session["IdCliente"] = id;
            Session["IdVolta"] = id;
            Session["VoltaCEP"] = 1;
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCliente(ClienteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TiposPessoa = new SelectList((List<TIPO_PESSOA>)Session["TiposPessoas"], "TIPE_CD_ID", "TIPE_NM_NOME");
            ViewBag.TiposCont = new SelectList(baseApp.GetAllContribuinte(idAss), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(idAss), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.Sexo = new SelectList(baseApp.GetAllSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Usuarios = new SelectList((List<USUARIO>)Session["Usuarios"], "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");

            List<SelectListItem> situacao = new List<SelectListItem>();
            situacao.Add(new SelectListItem() { Text = "Ativa", Value = "Ativa" });
            situacao.Add(new SelectListItem() { Text = "Inativa", Value = "Inativa" });
            situacao.Add(new SelectListItem() { Text = "Outros", Value = "Outros" });
            ViewBag.Situacoes = new SelectList(situacao, "Value", "Text");
            CLIENTE cli = baseApp.GetItemById(vm.CLIE_CD_ID);
            ViewBag.QuadroSoci = ccnpjApp.GetByCliente(cli);

            // Indicadores
            //ViewBag.Vendas = clie.PEDIDO_VENDA.Count;
            //ViewBag.Servicos = 0;
            //ViewBag.Atendimentos = clie.ATENDIMENTO.Count;
            ViewBag.Incluir = (Int32)Session["IncluirCliente"];

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirCliente"] = 0;

                    if (Session["FiltroCliente"] != null)
                    {
                        FiltrarCliente((CLIENTE)Session["FiltroCliente"]);
                    }

                    if ((Int32)Session["VoltaCliente"] == 2)
                    {
                        return RedirectToAction("VerCardsCliente");
                    }
                    if ((Int32)Session["VoltaCliente"]  == 3)
                    {
                        return RedirectToAction("VerClientesAtraso");
                    }
                    if ((Int32)Session["VoltaCliente"]  == 4)
                    {
                        return RedirectToAction("VerClientesSemPedidos");
                    }
                    if ((Int32)Session["VoltaCliente"]  == 5)
                    {
                        return RedirectToAction("VerClientesInativos");
                    }
                    if ((Int32)Session["VoltaCRM"] == 11)
                    {
                        return RedirectToAction("VoltarBaseCRM", "CRM");
                    }
                    return RedirectToAction("MontarTelaCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    vm = Mapper.Map<CLIENTE, ClienteViewModel>(cli);
                    return View(vm);
                }
            }
            else
            {
                vm = Mapper.Map<CLIENTE, ClienteViewModel>(cli);
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCliente(Int32 id)
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            CLIENTE item = baseApp.GetItemById(id);
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirCliente(ClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCliente"] = 4;
                    return RedirectToAction("MontarTelaCliente", "Cliente");
                }

                // Sucesso
                listaMaster = new List<CLIENTE>();
                Session["ListaCliente"] = null;
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        [HttpGet]
        public ActionResult ReativarCliente(Int32 id)
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
                    Session["MensCliente"] = 2;
                    return RedirectToAction("MontarTelaCliente");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Verifica possibilidade
            Int32 num = baseApp.GetAllItens(idAss).Count;
            if ((Int32)Session["NumClientes"] <= num)
            {
                Session["MensCliente"] = 50;
                return RedirectToAction("MontarTelaCliente", "Cliente");
            }

            // Prepara view
            CLIENTE item = baseApp.GetItemById(id);
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarCliente(ClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                Int32 volta = baseApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMaster = new List<CLIENTE>();
                Session["ListaCliente"] = null;
                return RedirectToAction("MontarTelaCliente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(objeto);
            }
        }

        public ActionResult VerCardsCliente()
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
            if ((List<CLIENTE>)Session["ListaCliente"] == null || ((List<CLIENTE>)Session["ListaCliente"]).Count == 0)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaCliente"] = listaMaster;
            }

            ViewBag.Listas = (List<CLIENTE>)Session["ListaCliente"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.UF = new SelectList((List<UF>)Session["UFs"], "UF_CD_ID", "UF_NM_NOME");
            Session["Cliente"] = null;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaCliente"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCliente"] != null)
            {
            }

            // Abre view
            Session["VoltaCliente"] = 2;
            objeto = new CLIENTE();
            return View(objeto);
        }

        [HttpGet]
        public ActionResult VerAnexoCliente(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarCliente", new { id = (Int32)Session["IdCliente"] });
        }

        public FileResult DownloadCliente(Int32 id)
        {
            CLIENTE_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.CLAN_AQ_ARQUIVO;
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

            Session["FileQueueCliente"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueCliente(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 50)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CLIENTE_ANEXO foto = new CLIENTE_ANEXO();
            foto.CLAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CLAN_DT_ANEXO = DateTime.Today;
            foto.CLAN_IN_ATIVO = 1;
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
            foto.CLAN_IN_TIPO = tipo;
            foto.CLAN_NM_TITULO = fileName;
            foto.CLIE_CD_ID = item.CLIE_CD_ID;

            item.CLIENTE_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFileCliente(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }

            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CLIENTE_ANEXO foto = new CLIENTE_ANEXO();
            foto.CLAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CLAN_DT_ANEXO = DateTime.Today;
            foto.CLAN_IN_ATIVO = 1;
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
            foto.CLAN_IN_TIPO = tipo;
            foto.CLAN_NM_TITULO = fileName;
            foto.CLIE_CD_ID = item.CLIE_CD_ID;

            item.CLIENTE_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueCliente(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }
            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.CLIE_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            listaMaster = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpPost]
        public ActionResult UploadFotoCliente(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCliente"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCliente"] = 5;
                return RedirectToAction("VoltarAnexoCliente");
            }
            CLIENTE item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensCliente"] = 6;
                return RedirectToAction("VoltarAnexoCliente");
            }
            String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.CLIE_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            listaMaster = new List<CLIENTE>();
            Session["ListaCliente"] = null;
            return RedirectToAction("VoltarAnexoCliente");
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
                hash.Add("CLIE_NM_ENDERECO", end.Address);
                hash.Add("CLIE_NR_NUMERO", end.Complement);
                hash.Add("CLIE_NM_BAIRRO", end.District);
                hash.Add("CLIE_NM_CIDADE", end.City);
                hash.Add("CLIE_SG_UF", end.Uf);
                hash.Add("UF_CD_ID", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("CLIE_NR_CEP", cep);
            }
            else if (tipoEnd == 2)
            {
                hash.Add("CLIE_NM_ENDERECO_ENTREGA", end.Address);
                hash.Add("CLIE_NR_NUMERO_ENTREGA", end.Complement);
                hash.Add("CLIE_NM_BAIRRO_ENTREGA", end.District);
                hash.Add("CLIE_NM_CIDADE_ENTREGA", end.City);
                hash.Add("CLIE_SG_UF_ENTREGA", end.Uf);
                hash.Add("UF_CD_ID_ENTREGA", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("CLIE_NR_CEP_ENTREGA", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public ActionResult PesquisaCEPEntrega(ClienteViewModel itemVolta)
        {
            // Chama servico ECT
            CLIENTE cli = baseApp.GetItemById((Int32)Session["IdCliente"]);
            ClienteViewModel item = Mapper.Map<CLIENTE, ClienteViewModel>(cli);

            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            String cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(itemVolta.CLIE_NR_CEP_BUSCA);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza            
            item.CLIE_NM_ENDERECO_ENTREGA = end.Address + "/" + end.Complement;
            item.CLIE_NM_BAIRRO_ENTREGA = end.District;
            item.CLIE_NM_CIDADE_ENTREGA = end.City;
            item.CLIE_SG_UF_ENTREGA = end.Uf;
            item.CLIE_UF_CD_ENTREGA = baseApp.GetUFbySigla(end.Uf).UF_CD_ID;
            item.CLIE_NR_CEP_ENTREGA = itemVolta.CLIE_NR_CEP_BUSCA;

            // Retorna
            Session["VoltaCEP"] = 2;
            Session["Cliente"] = Mapper.Map<ClienteViewModel, CLIENTE>(item);
            return RedirectToAction("BuscarCEPCliente2");
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(ClienteContatoViewModel vm)
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
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult ExcluirContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            item.CLCO_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE_CONTATO item = baseApp.GetContatoById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            item.CLCO_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditContato(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirContato()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            CLIENTE_CONTATO item = new CLIENTE_CONTATO();
            ClienteContatoViewModel vm = Mapper.Map<CLIENTE_CONTATO, ClienteContatoViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.CLCO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(ClienteContatoViewModel vm)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_CONTATO item = Mapper.Map<ClienteContatoViewModel, CLIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult EditarReferencia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            ClienteReferenciaViewModel vm = Mapper.Map<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarReferencia(ClienteReferenciaViewModel vm)
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
                    CLIENTE_REFERENCIA item = Mapper.Map<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>(vm);
                    Int32 volta = baseApp.ValidateEditReferencia(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult ExcluirReferencia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            item.CLRE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateEditReferencia(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult ReativarReferencia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CLIENTE_REFERENCIA item = baseApp.GetReferenciaById(id);
            objetoAntes = (CLIENTE)Session["Cliente"];
            item.CLRE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateEditReferencia(item);
            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirReferencia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CLIENTE_REFERENCIA item = new CLIENTE_REFERENCIA();
            ClienteReferenciaViewModel vm = Mapper.Map<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.CLRE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirReferencia(ClienteReferenciaViewModel vm)
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
                    CLIENTE_REFERENCIA item = Mapper.Map<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>(vm);
                    Int32 volta = baseApp.ValidateCreateReferencia(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult IncluirTag()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            List<SelectListItem> tipoTag = new List<SelectListItem>();
            tipoTag.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
            tipoTag.Add(new SelectListItem() { Text = "Alarme", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Elogio", Value = "2" });
            ViewBag.TipoTag = new SelectList(tipoTag, "Value", "Text");

            CLIENTE_TAG item = new CLIENTE_TAG();
            ClienteTagViewModel vm = Mapper.Map<CLIENTE_TAG, ClienteTagViewModel>(item);
            vm.CLIE_CD_ID = (Int32)Session["IdCliente"];
            vm.CLTA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTag(ClienteTagViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            List<SelectListItem> tipoTag = new List<SelectListItem>();
            tipoTag.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
            tipoTag.Add(new SelectListItem() { Text = "Alarme", Value = "1" });
            tipoTag.Add(new SelectListItem() { Text = "Elogio", Value = "2" });
            ViewBag.TipoTag = new SelectList(tipoTag, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE_TAG item = Mapper.Map<ClienteTagViewModel, CLIENTE_TAG>(vm);
                    Int32 volta = baseApp.ValidateCreateTag(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoCliente");
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
        public ActionResult VerClientesInativos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaCliente"] = 5;
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            if (Session["ListaInativos"] == null || ((List<CLIENTE>)Session["ListaInativos"]).Count == 0)
            {
                Session["ListaInativos"] = baseApp.GetAllItensAdm(idAss).Where(x => x.CLIE_IN_ATIVO == 0).ToList();
            }

            ViewBag.Listas = (List<CLIENTE>)Session["ListaInativos"];
            ViewBag.Title = "Clientes";
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "CACL_CD_ID", "CACL_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(idAss), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.UF = new SelectList((List<UF>)Session["UFs"], "UF_CD_ID", "UF_NM_NOME");
            Session["Cliente"] = null;

            // Indicadores
            ViewBag.Clientes = ((List<CLIENTE>)Session["ListaInativos"]).Count;
            //SessionMocks.listaCR = crApp.GetItensAtrasoCliente().ToList();
            //ViewBag.Atrasos = SessionMocks.listaCR.Select(x => x.CLIE_CD_ID).Distinct().ToList().Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensClienteInativos"] != null)
            {
            }

            // Abre view
            objeto = new CLIENTE();
            Session["VoltaCliente"] = 1;
            return View(objeto);
        }

        [HttpPost]
        public ActionResult FiltrarInativos(CLIENTE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CLIENTE> listaObj = new List<CLIENTE>();
                Int32 volta = baseApp.ExecuteFilter(null, null, null, item.CLIE_NM_NOME, null, null, null, item.CLIE_NM_CIDADE, item.UF_CD_ID, 0, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    return RedirectToAction("VerClientesInativos");
                }

                // Sucesso
                listaMaster = listaObj;
                Session["ListaInativos"] = listaObj;
                return RedirectToAction("VerClientesInativos");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VerClientesInativos");
            }
        }

        public ActionResult RetirarFiltroInativos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaInativos"] = null;
            return RedirectToAction("VerClientesInativos");
        }

        public ActionResult GerarRelatorioLista()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ClienteLista" + "_" + data + ".pdf";
            List<CLIENTE> lista = (List<CLIENTE>)Session["ListaCliente"];
            CLIENTE filtro = (CLIENTE)Session["FiltroCliente"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Clientes - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Clientes selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
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
            cell = new PdfPCell(new Paragraph("CPF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CNPJ", meuFont))
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
            cell = new PdfPCell(new Paragraph("Cidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CLIENTE item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_CLIENTE.CACL_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CLIE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIE_NR_CPF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CPF, meuFont))
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
                if (item.CLIE_NR_CNPJ != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_CNPJ, meuFont))
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
                cell = new PdfPCell(new Paragraph(item.CLIE_NM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CLIE_NR_TELEFONE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NR_TELEFONE, meuFont))
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
                if (item.CLIE_NM_CIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CLIE_NM_CIDADE, meuFont))
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
                if (item.UF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.UF.UF_SG_SIGLA, meuFont))
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
                if (filtro.CACL_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CACL_CD_ID;
                    ja = 1;
                }
                if (filtro.CLIE_CD_ID > 0)
                {
                    CLIENTE cli = baseApp.GetItemById(filtro.CLIE_CD_ID);
                    if (ja == 0)
                    {
                        parametros += "Nome: " + cli.CLIE_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + cli.CLIE_NM_NOME;
                    }
                }
                if (filtro.CLIE_NR_CPF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CPF: " + filtro.CLIE_NR_CPF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.CLIE_NR_CPF;
                    }
                }
                if (filtro.CLIE_NR_CNPJ != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CNPJ: " + filtro.CLIE_NR_CNPJ;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CNPJ: " + filtro.CLIE_NR_CNPJ;
                    }
                }
                if (filtro.CLIE_NM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: " + filtro.CLIE_NM_EMAIL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: " + filtro.CLIE_NM_EMAIL;
                    }
                }
                if (filtro.CLIE_NM_CIDADE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Cidade: " + filtro.CLIE_NM_CIDADE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Cidade: " + filtro.CLIE_NM_CIDADE;
                    }
                }
                if (filtro.UF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
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

            return RedirectToAction("MontarTelaCliente");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            // Prepara geração
            CLIENTE aten = baseApp.GetItemById((Int32)Session["IdCliente"]);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Cliente" + aten.CLIE_CD_ID.ToString() + "_" + data + ".pdf";
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/favicon_SystemBR.jpg"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Cliente - Detalhes", meuFont2))
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

            cell = new PdfPCell(new Paragraph("Foto", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            try
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                Image imagemCliente = Image.GetInstance(Server.MapPath(aten.CLIE_AQ_FOTO));
                imagemCliente.ScaleAbsolute(50, 50);
                cell.AddElement(imagemCliente);
                table.AddCell(cell);
            }
            catch (Exception ex)
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                Image imagemCliente = Image.GetInstance(Server.MapPath("~/Images/a8.jpg"));
                imagemCliente.ScaleAbsolute(50, 50);
                cell.AddElement(imagemCliente);
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Tipo de Pessoa: " + aten.TIPO_PESSOA.TIPE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Filial: " + aten.FILIAL.FILI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.TIPO_CONTRIBUINTE != null)
            {
                cell = new PdfPCell(new Paragraph("Tipo Contribuinte: " + aten.TIPO_CONTRIBUINTE.TICO_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Tipo Contribuinte: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_CLIENTE.CACL_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + aten.CLIE_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Razão Social: " + aten.CLIE_NM_RAZAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CLIE_NR_CPF != null)
            {
                cell = new PdfPCell(new Paragraph("CPF: " + aten.CLIE_NR_CPF, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.CLIE_NR_CNPJ != null)
            {
                cell = new PdfPCell(new Paragraph("CNPJ: " + aten.CLIE_NR_CNPJ, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ins.Estadual: " + aten.CLIE_NR_INSCRICAO_ESTADUAL, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ins.Municipal: " + aten.CLIE_NR_INSCRICAO_MUNICIPAL, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.CLIE_VL_SALDO != null)
                {
                    cell = new PdfPCell(new Paragraph("Saldo: " + CrossCutting.Formatters.DecimalFormatter(aten.CLIE_VL_SALDO.Value), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Saldo: 0,00", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
            }

            if (aten.REGIME_TRIBUTARIO != null)
            {
                cell = new PdfPCell(new Paragraph("Regime Tributário: " + aten.REGIME_TRIBUTARIO.RETR_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Regime Tributário: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("Ins.SUFRAMA: " + aten.CLIE_NR_SUFRAMA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Endereços
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Endereço Principal", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIE_NM_ENDERECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número: " + aten.CLIE_NR_NUMERO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Complemento: " + aten.CLIE_NM_COMPLEMENTO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Bairro: " + aten.CLIE_NM_BAIRRO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade: " + aten.CLIE_NM_CIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.UF != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("CEP: " + aten.CLIE_NR_CEP, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço de Entrega", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Endereço: " + aten.CLIE_NM_ENDERECO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número: " + aten.CLIE_NR_NUMERO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Complemento: " + aten.CLIE_NM_COMPLEMENTO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Bairro: " + aten.CLIE_NM_BAIRRO_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade: " + aten.CLIE_NM_CIDADE_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            if (aten.UF1 != null)
            {
                cell = new PdfPCell(new Paragraph("UF: " + aten.UF1.UF_SG_SIGLA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("UF: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph("CEP: " + aten.CLIE_NR_CEP_ENTREGA, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Contatos
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Contatos", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("E-Mail: " + aten.CLIE_NM_EMAIL, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail DANFE: " + aten.CLIE_NM_EMAIL_DANFE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Redes Sociais: " + aten.CLIE_NM_REDES_SOCIAIS, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Website: " + aten.CLIE_NM_WEBSITE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Telefone: " + aten.CLIE_NR_TELEFONE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular: " + aten.CLIE_NR_CELULAR, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Tel.Adicional: " + aten.CLIE_NR_TELEFONE_ADICIONAL, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Lista de Contatos
            if (aten.CLIENTE_CONTATO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 100f, 120f, 100f, 50f });
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
                cell = new PdfPCell(new Paragraph("Cargo", meuFont))
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

                foreach (CLIENTE_CONTATO item in aten.CLIENTE_CONTATO)
                {
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_CARGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CLCO_NM_TELEFONE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.CLCO_IN_ATIVO == 1)
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

            // Dados Pessoais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Pessoais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome do Pai: " + aten.CLIE_NM_PAI, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome da Mãe: " + aten.CLIE_NM_MAE, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.CLIE_DT_NASCIMENTO != null)
            {
                cell = new PdfPCell(new Paragraph("Data Nascimento: " + aten.CLIE_DT_NASCIMENTO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data Nascimento: ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.SEXO != null)
            {
                cell = new PdfPCell(new Paragraph("Sexo: " + aten.SEXO.SEXO_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Sexo: - ", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Naturalidade: " + aten.CLIE_NM_NATURALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF Naturalidade: " + aten.CLIE_SG_NATURALIADE_UF, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nacionalidade: " + aten.CLIE_NM_NACIONALIDADE, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph(" ", meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Dados Comerciais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Comerciais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.USUARIO != null)
            {
                cell = new PdfPCell(new Paragraph("Vendedor: " + aten.USUARIO.USUA_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Vendedor: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.CLIE_VL_LIMITE_CREDITO != null)
            {
                cell = new PdfPCell(new Paragraph("Limite de Crédito: " + CrossCutting.Formatters.DecimalFormatter(aten.CLIE_VL_LIMITE_CREDITO.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Limite de Crédito: 0,00", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.CLIE_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Pedidos de Venda
            if (aten.PEDIDO_VENDA.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                cell = new PdfPCell(new Paragraph("Pedidos de Venda", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                // Lista de Pedidos
                table = new PdfPTable(new float[] { 120f, 80f, 80f, 80f });
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
                cell = new PdfPCell(new Paragraph("Emissão", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Status", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Aprovação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PEDIDO_VENDA item in aten.PEDIDO_VENDA)
                {
                    cell = new PdfPCell(new Paragraph(item.PEVE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PEVE_DT_DATA.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PEVE_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Emissão", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Em Aprovação", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Aprovado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Cancelado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Encerrado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PEVE_DT_APROVACAO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PEVE_DT_APROVACAO.Value.ToShortDateString(), meuFont))
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

            // Atendimento
            if (aten.PEDIDO_VENDA.Count > 0)
            {
                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                cell = new PdfPCell(new Paragraph("Pedidos de Venda", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                // Lista de Pedidos
                table = new PdfPTable(new float[] { 120f, 80f, 80f, 80f });
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
                cell = new PdfPCell(new Paragraph("Emissão", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Status", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Aprovação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PEDIDO_VENDA item in aten.PEDIDO_VENDA)
                {
                    cell = new PdfPCell(new Paragraph(item.PEVE_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PEVE_DT_DATA.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PEVE_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Emissão", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Em Aprovação", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Aprovado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Cancelado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PEVE_IN_STATUS == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Encerrado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PEVE_DT_APROVACAO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PEVE_DT_APROVACAO.Value.ToShortDateString(), meuFont))
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

            return RedirectToAction("VoltarAnexoCliente");
        }

        [HttpGet]
        public ActionResult IncluirClienteRapido()
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

            // Prepara listas
            ViewBag.TiposPessoa = new SelectList(baseApp.GetAllTiposPessoa().OrderBy(p => p.TIPE_NM_NOME), "TIPE_CD_ID", "TIPE_NM_NOME");
            Session["Cliente"] = null;
            ViewBag.Sexo = new SelectList(baseApp.GetAllSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.TiposCont = new SelectList(baseApp.GetAllContribuinte(idAss), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(idAss), "RETR_CD_ID", "RETR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Oportunidade", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta", Value = "3" });
            status.Add(new SelectListItem() { Text = "Engajado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Descartado", Value = "5" });
            status.Add(new SelectListItem() { Text = "Suspenso", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            if (Session["MensCliente"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCliente"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0021", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            Session["ClienteNovo"] = 0;
            Session["VoltaCliente"] = 3;
            CLIENTE item = new CLIENTE();
            ClienteViewModel vm = Mapper.Map<CLIENTE, ClienteViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.CACL_CD_ID = 1;
            vm.CLIE_AQ_FOTO = "~/Images/icone_imagem.jpg";
            vm.CLIE_DT_CADASTRO = DateTime.Today.Date;
            vm.CLIE_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.CLIE_IN_STATUS = 1;
            vm.TIPE_CD_ID = 0;
            vm.FILI_CD_ID = usuario.FILI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirClienteRapido(ClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.TiposPessoa = new SelectList(baseApp.GetAllTiposPessoa().OrderBy(p => p.TIPE_NM_NOME), "TIPE_CD_ID", "TIPE_NM_NOME");
            Session["Cliente"] = null;
            ViewBag.Sexo = new SelectList(baseApp.GetAllSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.TiposCont = new SelectList(baseApp.GetAllContribuinte(idAss), "TICO_CD_ID", "TICO_NM_NOME");
            ViewBag.Regimes = new SelectList(baseApp.GetAllRegimes(idAss), "RETR_CD_ID", "RETR_NM_NOME");
            ViewBag.UF = new SelectList(baseApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Prospecção", Value = "1" });
            status.Add(new SelectListItem() { Text = "Oportunidade", Value = "2" });
            status.Add(new SelectListItem() { Text = "Proposta", Value = "3" });
            status.Add(new SelectListItem() { Text = "Engajado", Value = "4" });
            status.Add(new SelectListItem() { Text = "Descartado", Value = "5" });
            status.Add(new SelectListItem() { Text = "Suspenso", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CLIENTE item = Mapper.Map<ClienteViewModel, CLIENTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCliente"] = 10;
                        return RedirectToAction("IncluirClienteRapido", "Cliente");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Clientes/" + item.CLIE_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<CLIENTE>();
                    Session["ListaCliente"] = null;
                    Session["IncluirCliente"] = 1;
                    Session["ClienteNovo"] = item.CLIE_CD_ID;

                    if (item.TIPE_CD_ID == 2)
                    {
                        var lstQs = PesquisaCNPJ(item);

                        foreach (var qs in lstQs)
                        {
                            Int32 voltaQs = ccnpjApp.ValidateCreate(qs, usuario);
                        }
                    }

                    if ((Int32)Session["VoltaCliente"] == 3)
                    {
                        Session["VoltaCliente"] = 0;
                        return RedirectToAction("IncluirClienteRapido", "Cliente");
                    }
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                vm.TIPE_CD_ID = 0;
                return View(vm);
            }
        }
    }
}