using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class TransportadoraRepository : RepositoryBase<TRANSPORTADORA>, ITransportadoraRepository
    {
        public TRANSPORTADORA CheckExist(TRANSPORTADORA conta, Int32 idAss)
        {
            IQueryable<TRANSPORTADORA> query = Db.TRANSPORTADORA;
            query = query.Where(p => p.TRAN_NM_NOME == conta.TRAN_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TRANSPORTADORA GetByEmail(String email, Int32 idAss)
        {
            IQueryable<TRANSPORTADORA> query = Db.TRANSPORTADORA.Where(p => p.TRAN_IN_ATIVO == 1);
            query = query.Where(p => p.TRAN_NM_EMAIL == email);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TRANSPORTADORA GetItemById(Int32 id)
        {
            IQueryable<TRANSPORTADORA> query = Db.TRANSPORTADORA;
            query = query.Where(p => p.TRAN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TRANSPORTADORA> GetAllItens(Int32 idAss)
        {
            IQueryable<TRANSPORTADORA> query = Db.TRANSPORTADORA.Where(p => p.TRAN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TRANSPORTADORA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TRANSPORTADORA> query = Db.TRANSPORTADORA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TRANSPORTADORA> ExecuteFilter(Int32? veic, Int32? tran, String nome, String cnpj, String email, String cidade, String uf, Int32 idAss)
        {
            List<TRANSPORTADORA> lista = new List<TRANSPORTADORA>();
            IQueryable<TRANSPORTADORA> query = Db.TRANSPORTADORA;
            if (veic != null)
            {
                query = query.Where(p => p.TIVE_CD_ID == veic);
            }
            if (tran != null)
            {
                query = query.Where(p => p.TITR_CD_ID == tran);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TRAN_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.TRAN_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.TRAN_NM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.TRAN_NM_CIDADE.Contains(cidade));
            }
            if (!String.IsNullOrEmpty(uf))
            {
                query = query.Where(p => p.TRAN_SG_UF ==uf);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.TRAN_NM_NOME);
                lista = query.ToList<TRANSPORTADORA>();
            }
            return lista;
        }
    }
}
