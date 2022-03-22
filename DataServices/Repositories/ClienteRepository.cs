using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class ClienteRepository : RepositoryBase<CLIENTE>, IClienteRepository
    {
        public CLIENTE CheckExist(CLIENTE conta, Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            query = query.Where(p => p.CLIE_NM_NOME == conta.CLIE_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CLIENTE GetByEmail(String email)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE.Where(p => p.CLIE_IN_ATIVO == 1);
            query = query.Where(p => p.CLIE_NM_EMAIL == email);
            query = query.Include(p => p.CLIENTE_ANEXO);
            query = query.Include(p => p.CLIENTE_CONTATO);
            query = query.Include(p => p.CLIENTE_TAG);
            return query.FirstOrDefault();
        }

        public CLIENTE GetItemById(Int32 id)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            query = query.Where(p => p.CLIE_CD_ID == id);
            query = query.Include(p => p.CLIENTE_ANEXO);
            query = query.Include(p => p.CLIENTE_CONTATO);
            query = query.Include(p => p.CLIENTE_TAG);
            return query.FirstOrDefault();
        }

        public List<CLIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE.Where(p => p.CLIE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CLIE_NM_NOME);
            return query.ToList();
        }

        public List<CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CLIENTE> query = Db.CLIENTE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.CLIE_NM_NOME);
            return query.ToList();
        }

        public List<CLIENTE> ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32 idAss)
        {
            List<CLIENTE> lista = new List<CLIENTE>();
            IQueryable<CLIENTE> query = Db.CLIENTE;
            if (id != 0)
            {
                query = query.Where(p => p.CLIE_CD_ID == id);
            }
            if (catId != null)
            {
                query = query.Where(p => p.CATEGORIA_CLIENTE.CACL_CD_ID == catId);
            }
            if (ativo != null)
            {
                query = query.Where(p => p.CLIE_IN_ATIVO == ativo);
            }
            else
            {
                query = query.Where(p => p.CLIE_IN_ATIVO == 1);
            }
            if (!String.IsNullOrEmpty(razao))
            {
                query = query.Where(p => p.CLIE_NM_RAZAO.Contains(razao));
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.CLIE_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.CLIE_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                cnpj = ValidarNumerosDocumentos.RemoveNaoNumericos(cnpj);
                query = query.Where(p => p.CLIE_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.CLIE_NM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.CLIE_NM_CIDADE.Contains(cidade));
            }
            if (uf != null)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CLIE_NM_NOME);
                lista = query.ToList<CLIENTE>();
            }
            return lista;
        }

        //public List<CLIENTE> ExecuteFilterSemPedido(String nome, String cidade, Int32? uf)
        //{
        //    Int32? idAss = SessionMocks.IdAssinante;
        //    List<CLIENTE> lista = new List<CLIENTE>();
        //    IQueryable<CLIENTE> query = Db.CLIENTE.Where(p => p.PEDIDO_VENDA.Count == 0);
        //    if (!String.IsNullOrEmpty(nome))
        //    {
        //        query = query.Where(p => p.CLIE_NM_NOME.Contains(nome));
        //    }
        //    if (!String.IsNullOrEmpty(cidade))
        //    {
        //        query = query.Where(p => p.CLIE_NM_CIDADE.Contains(cidade));
        //    }
        //    if (uf != null)
        //    {
        //        query = query.Where(p => p.UF_CD_ID == uf);
        //    }
        //    if (query != null)
        //    {
        //        query = query.Where(p => p.ASSI_CD_ID == idAss);
        //        lista = query.ToList<CLIENTE>();
        //    }
        //    return lista;
        //}

        public List<CLIENTE> FiltrarContatos(MontagemGrupo grupo, Int32 idAss)
        {
            List<CLIENTE> lista = new List<CLIENTE>();
            IQueryable<CLIENTE> query = Db.CLIENTE;
            if (grupo.SEXO != null)
            {
                query = query.Where(p => p.SEXO_CD_ID == grupo.SEXO);
            }
            if (grupo.CATEGORIA != null)
            {
                query = query.Where(p => p.CATEGORIA_CLIENTE.CACL_CD_ID == grupo.CATEGORIA);
            }
            if (!String.IsNullOrEmpty(grupo.CIDADE))
            {
                query = query.Where(p => p.CLIE_NM_CIDADE.Contains(grupo.CIDADE));
            }
            if (grupo.UF != null)
            {
                query = query.Where(p => p.UF_CD_ID == grupo.UF);
            }
            if (grupo.DATA_NASC != null)
            {
                query = query.Where(p => p.CLIE_DT_NASCIMENTO == grupo.DATA_NASC);
            }
            else
            {
                if (grupo.DIA != null)
                {
                    Int32 dia = Convert.ToInt32(grupo.DIA);
                    query = query.Where(p => DbFunctions.TruncateTime(p.CLIE_DT_NASCIMENTO).Value.Day == dia);
                }
                if (grupo.MES != null)
                {
                    Int32 mes = Convert.ToInt32(grupo.MES);
                    query = query.Where(p => DbFunctions.TruncateTime(p.CLIE_DT_NASCIMENTO).Value.Month == mes);
                }
                if (grupo.ANO != null)
                {
                    Int32 ano = Convert.ToInt32(grupo.ANO);
                    query = query.Where(p => DbFunctions.TruncateTime(p.CLIE_DT_NASCIMENTO).Value.Year == ano);
                }
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.CLIE_NM_NOME);
                lista = query.ToList<CLIENTE>();
            }
            return lista;
        }

    }
}
 