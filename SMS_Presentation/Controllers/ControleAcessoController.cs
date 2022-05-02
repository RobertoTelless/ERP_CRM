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

namespace ERP_CRM_Solution.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public ControleAcessoController(IUsuarioAppService baseApps)
        {
            baseApp = baseApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult Login()
        {
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario;
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                USUARIO login = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateLogin(login.USUA_NM_LOGIN, login.USUA_NM_SENHA, out usuario);
                if (volta == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 11)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Armazena credenciais para autorização
                Session["UserCredentials"] = usuario;
                Session["Usuario"] = usuario;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["TipoAssinante"] = usuario.ASSINANTE.ASSI_IN_TIPO.Value;
                Session["Assinante"] = usuario.ASSINANTE;

                // Atualiza view
                String frase = String.Empty;
                String nome = usuario.USUA_NM_NOME;
                if (DateTime.Now.Hour <= 12)
                {
                    frase = "Bom dia, " + nome;
                }
                else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
                {
                    frase = "Boa tarde, " + nome;
                }
                else
                {
                    frase = "Boa noite, " + nome;
                }

                ViewBag.Greeting = frase;
                ViewBag.Nome = usuario.USUA_NM_NOME;
                if (usuario.CARGO != null)
                {
                    ViewBag.Cargo = usuario.CARGO.CARG_NM_NOME;
                    Session["Cargo"] = usuario.CARGO.CARG_NM_NOME;
                }
                else
                {
                    ViewBag.Cargo = "-";
                    Session["Cargo"] = "-";
                }
                ViewBag.Foto = usuario.USUA_AQ_FOTO;

                Session["Greeting"] = frase;
                Session["Nome"] = usuario.USUA_NM_NOME;
                Session["Foto"] = usuario.USUA_AQ_FOTO;
                Session["Perfil"] = usuario.PERFIL;
                Session["PerfilSigla"] = usuario.PERFIL.PERF_SG_SIGLA;
                Session["PerfilSistema"] = usuario.USUA_IN_SISTEMA;
                Session["FlagInicial"] = 0;
                Session["FiltroData"] = 1;
                Session["FiltroStatus"] = 1;
                Session["Ativa"] = "1";
                Session["Login"] = 1;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["IdFilial"] = usuario.FILI_CD_ID;
                Session["IdUsuario"] = usuario.USUA_CD_ID;

                // Recupera Planos do assinante
                Session["PermMens"] = 0;
                Session["PermCRM"] = 0;
                Session["PermServDesk"] = 0;
                Session["PermFatura"] = 0;
                Session["PermCompra"] = 0;
                Session["PermFinanceiro"] = 0;
                Session["PermEstoque"] = 0;
                Session["PermPatrimonio"] = 0;
                Session["PermVenda"] = 0;
                Session["NumSMS"] = 0;
                Session["NumEMail"] = 0;
                Session["NumZap"] = 0;
                Session["NumClientes"] = 0;
                Session["NumProcessos"] = 0;
                Session["NumPatrimonios"] = 0;
                Session["NumProdutos"] = 0;
                Session["NumFornecedor"] = 0;
                Session["NumAtendimentos"] = 0;
                Session["NumOrdemServico"] = 0;
                Session["NumUsuarios"] = 0;
                List<ASSINANTE_PLANO> plAss = usuario.ASSINANTE.ASSINANTE_PLANO.ToList();
                List<PLANO> planos = new List<PLANO>();
                foreach (var item in plAss)
                {
                    planos.Add(item.PLANO);
                    Session["NumUsuarios"] = item.PLANO.PLAN_NR_USUARIOS;
                    if (item.PLANO.PLAN_IN_MENSAGENS == 1)
                    {
                        Session["PermMens"] = 1;
                        Session["NumSMS"] = item.PLANO.PLAN_NR_SMS;
                        Session["NumEMail"] = item.PLANO.PLAN_NR_EMAIL;
                        Session["NumZap"] = item.PLANO.PLAN_NR_WHATSAPP;
                        Session["NumClientes"] = item.PLANO.PLAN_NR_CONTATOS;
                    }
                    if (item.PLANO.PLAN_IN_CRM == 1)
                    {
                        Session["PermCRM"] = 1;
                        Session["NumProc"] = item.PLANO.PLAN_NR_PROCESSOS;
                        Session["NumAcoes"] = item.PLANO.PLAN_NR_ACOES;
                        Session["NumClientes"] = item.PLANO.PLAN_NR_CONTATOS;
                    }
                    if (item.PLANO.PLAN_IN_PATRIMONIO == 1)
                    {
                        Session["PermPatrimonio"] = 1;
                        Session["NumFornecedor"] = item.PLANO.PLAN_NR_FORNECEDOR;
                        Session["NumPatrimonios"] = item.PLANO.PLAN_NR_PATRIMONIO;
                    }
                    if (item.PLANO.PLAN_IN_POSVENDA == 1)
                    {
                        Session["PermPosVenda"] = 1;
                    }
                    if (item.PLANO.PLAN_IN_FATURA == 1)
                    {
                        Session["PermFatura"] = 1;
                    }
                    if (item.PLANO.PLAN_IN_FINANCEIRO == 1)
                    {
                        Session["PermFinanceiro"] = 1;
                    }
                    if (item.PLANO.PLAN_IN_ESTOQUE == 1)
                    {
                        Session["PermEstoque"] = 1;
                        Session["NumFornecedor"] = item.PLANO.PLAN_NR_FORNECEDOR;
                        Session["NumProduto"] = item.PLANO.PLAN_NR_PRODUTO;
                    }
                    if (item.PLANO.PLAN_IN_COMPRA == 1)
                    {
                        Session["PermCompra"] = 1;
                        Session["NumFornecedor"] = item.PLANO.PLAN_NR_FORNECEDOR;
                        Session["NumProduto"] = item.PLANO.PLAN_NR_PRODUTO;
                        Session["NumCompra"] = item.PLANO.PLAN_NR_COMPRA;
                    }
                    if (item.PLANO.PLAN_IN_VENDAS == 1)
                    {
                        Session["PermVenda"] = 1;
                        Session["NumProduto"] = item.PLANO.PLAN_NR_PRODUTO;
                        Session["NumClientes"] = item.PLANO.PLAN_NR_CONTATOS;
                        Session["NumVendas"] = item.PLANO.PLAN_NR_VENDA;
                    }
                    if (item.PLANO.PLAN_IN_POSVENDA == 1)
                    {
                        Session["PermServDesk"] = 1;
                        Session["NumAtendimentos"] = item.PLANO.PLAN_IN_ATENDIMENTOS;
                        Session["NumOrdemServico"] = item.PLANO.PLAN_IN_OS;
                        Session["NumServicos"] = item.PLANO.PLAN_IN_SERVICOS;
                        Session["NumClientes"] = item.PLANO.PLAN_NR_CONTATOS;
                    }
                }
                Session["Planos"] = planos;

                // Se perfil CRM
                if (usuario.PERFIL.PERF_SG_SIGLA == "CRM")
                {
                    Session["PermMens"] = 0;
                    Session["PermPatrimonio"] = 0;
                    Session["PermPosVenda"] = 0;
                    Session["PermFinanceiro"] = 0;
                    Session["PermEstoque"] = 0;
                    Session["PermCompra"] = 0;
                    Session["PermVenda"] = 0;
                    Session["PermServDesk"] = 0;
                }
                Session["ListaMensagem"] = null;

                // Route
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    return RedirectToAction("TrocarSenhaInicio", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        public ActionResult Logout()
        {
            Session["UserCredentials"] = null;
            Session.Clear();
            return RedirectToAction("Login", "ControleAcesso");
        }

        public ActionResult Cancelar()
        {
            return RedirectToAction("Login", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult TrocarSenha()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = PlatMensagens_Resources.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                Session["UserCredentials"] = null;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaInicio()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Reseta senhas
            usuario.USUA_NM_NOVA_SENHA = null;
            usuario.USUA_NM_SENHA_CONFIRMA = null;
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                // Checa credenciais e atualiza acessos
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0074", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                ViewBag.Message = PlatMensagens_Resources.ResourceManager.GetString("M0075", CultureInfo.CurrentCulture);
                Session["UserCredentials"] = null;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult GerarSenha()
        {
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GerarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                // Processa
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.GenerateNewPassword(item.USUA_NM_EMAIL);
                if (volta == 1)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if (volta == 2)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                }
                if (volta == 3)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if (volta == 4)
                {
                    return Json(PlatMensagens_Resources.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
                return Json(1);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Json(ex.Message);
            }
        }
    }
}