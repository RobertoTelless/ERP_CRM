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
    public class TransportadoraService : ServiceBase<TRANSPORTADORA>, ITransportadoraService
    {
        private readonly ITransportadoraRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITransportadoraAnexoRepository _anexoRepository;
        private readonly IFilialRepository _filialRepository;
        private readonly ITipoVeiculoRepository _veicRepository;
        private readonly ITipoTransporteRepository _transRepository;
        private readonly IUFRepository _ufRepository;
        protected DUO_DatabaseEntities Db = new DUO_DatabaseEntities();

        public TransportadoraService(ITransportadoraRepository baseRepository, ILogRepository logRepository, ITransportadoraAnexoRepository anexoRepository, IFilialRepository filialRepository, ITipoVeiculoRepository veicRepository, ITipoTransporteRepository transRepository, IUFRepository ufRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _filialRepository = filialRepository;
            _veicRepository = veicRepository;
            _transRepository = transRepository;
            _ufRepository = ufRepository;
        }

        public TRANSPORTADORA CheckExist(TRANSPORTADORA conta, Int32 idAss)
        {
            TRANSPORTADORA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _ufRepository.GetItemBySigla(sigla);
        }

        public TRANSPORTADORA GetItemById(Int32 id)
        {
            TRANSPORTADORA item = _baseRepository.GetItemById(id);
            return item;
        }

        public TRANSPORTADORA GetByEmail(String email, Int32 idAss)
        {
            TRANSPORTADORA item = _baseRepository.GetByEmail(email, idAss);
            return item;
        }

        public List<TRANSPORTADORA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<TRANSPORTADORA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            return _filialRepository.GetAllItens(idAss);
        }

        public List<TIPO_VEICULO> GetAllTipoVeiculo(Int32 idAss)
        {
            return _veicRepository.GetAllItens(idAss);
        }

        public List<TIPO_TRANSPORTE> GetAllTipoTransporte(Int32 idAss)
        {
            return _transRepository.GetAllItens(idAss);
        }

        public TRANSPORTADORA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<TRANSPORTADORA> ExecuteFilter(Int32? veic, Int32? tran, String nome, String cnpj, String email, String cidade, String uf, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(veic, tran, nome, cnpj, email, cidade, uf, idAss);

        }

        public Int32 Create(TRANSPORTADORA item, LOG log)
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

        public Int32 Create(TRANSPORTADORA item)
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


        public Int32 Edit(TRANSPORTADORA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TRANSPORTADORA obj = _baseRepository.GetById(item.TRAN_CD_ID);
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

        public Int32 Edit(TRANSPORTADORA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    TRANSPORTADORA obj = _baseRepository.GetById(item.TRAN_CD_ID);
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

        public Int32 Delete(TRANSPORTADORA item, LOG log)
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
    }
}
