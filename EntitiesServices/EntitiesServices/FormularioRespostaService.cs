using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class FormularioRespostaService : ServiceBase<FORMULARIO_RESPOSTA>, IFormularioRespostaService
    {
        private readonly IFormularioRespostaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IUFRepository _ufRepository;
        private readonly IFormularioRespostaAcaoRepository _acaRepository;
        private readonly IFormularioRespostaAnexoRepository _aneRepository;
        private readonly IFormularioRespostaComentarioRepository _comRepository;

        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public FormularioRespostaService(IFormularioRespostaRepository baseRepository, ILogRepository logRepository, IUFRepository ufRepository, IFormularioRespostaAcaoRepository acaRepository, IFormularioRespostaAnexoRepository aneRepository, IFormularioRespostaComentarioRepository comRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _ufRepository = ufRepository;
            _acaRepository = acaRepository;
            _aneRepository = aneRepository;
            _comRepository = comRepository;
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _ufRepository.GetItemBySigla(sigla);
        }

        public FORMULARIO_RESPOSTA GetItemById(Int32 id)
        {
            FORMULARIO_RESPOSTA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<FORMULARIO_RESPOSTA> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<FORMULARIO_RESPOSTA> ExecuteFilter(Int32? status, String nome, String email, String cidade, String celular, Int32? uf)
        {
            return _baseRepository.ExecuteFilter(status, nome, email, celular, cidade, uf);
        }

        public Int32 Create(FORMULARIO_RESPOSTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(FORMULARIO_RESPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(FORMULARIO_RESPOSTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORMULARIO_RESPOSTA obj = _baseRepository.GetById(item.FORE_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(FORMULARIO_RESPOSTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORMULARIO_RESPOSTA obj = _baseRepository.GetById(item.FORE_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(FORMULARIO_RESPOSTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<FORMULARIO_RESPOSTA_ACAO> GetAllAcoes()
        {
            return _acaRepository.GetAllItens();
        }

        public FORMULARIO_RESPOSTA_ANEXO GetAnexoById(Int32 id)
        {
            return _aneRepository.GetItemById(id);
        }

        public FORMULARIO_RESPOSTA_COMENTARIO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public FORMULARIO_RESPOSTA_ACAO GetAcaoById(Int32 id)
        {
            return _acaRepository.GetItemById(id);
        }

        public Int32 CreateAcao(FORMULARIO_RESPOSTA_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _acaRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditAcao(FORMULARIO_RESPOSTA_ACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORMULARIO_RESPOSTA_ACAO obj = _acaRepository.GetById(item.FRAC_CD_ID);
                    _acaRepository.Detach(obj);
                    _acaRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }



    }
}
