using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace SystemBRPresentation.ViewModels
{
    public class OrdemServicoViewModel
    {
        [Key]
        public int ORSE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> ATEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA CRIACAO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime ORSE_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo DATA INICIO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime ORSE_DT_INICIO { get; set; }
        [Required(ErrorMessage = "Campo DATA INICIO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime ORSE_DT_PREVISTA { get; set; }
        public string ORSE_NR_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "A NOTA FISCAL deve conter no máximo 20.")]
        public string ORSE_NR_NOTA_FISCAL { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ORSE_DT_CANCELAMENTO { get; set; }
        [StringLength(1000, ErrorMessage = "MOTIVO DE CANCELAMENTO deve conter no máximo 1000.")]
        public string ORSE_DS_MOTIVO_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> ORSE_DT_ENCERRAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "O ENCERRAMENTO deve conter no máximo 5000.")]
        public string ORSE_DS_ENCERRAMENTO { get; set; }
        public Nullable<int> CLIE_CD_ID { get; set; }
        public int ORSE_IN_VISITA { get; set; }
        public int ORSE_IN_ATIVO { get; set; }
        public int ORSE_IN_STATUS { get; set; }
        public string ORSE_TX_OBSERVACOES { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }
        public Nullable<int> SERV_CD_ID { get; set; }
        public Nullable<int> CAOS_CD_ID { get; set; }
        public int DEPT_CD_ID { get; set; }
        public string ORSE_DS_DESCRICAO { get; set; }
        public Nullable<int> ORSE_IN_VENDEDOR { get; set; }
        public string ORSE_NM_EQUIPAMENTO { get; set; }
        public string ORSE_NR_EQUIPAMENTO { get; set; }
        public Nullable<int> ORSE_IN_TECNICO { get; set; }
        public Nullable<int> ORSE_IN_ORCAMENTO { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }

        public string item_obs { get; set; }
        public int qtde { get; set; }
        public decimal preco { get; set; }
        public decimal promo { get; set; }

        public bool Orcar
        {
            get
            {
                if (ORSE_IN_ORCAMENTO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                ORSE_IN_ORCAMENTO = value ? 1 : 0;
            }
        }

        public virtual ATENDIMENTO ATENDIMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual SERVICO SERVICO { get; set; }
        public virtual DEPARTAMENTO DEPARTAMENTO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        public virtual CATEGORIA_ORDEM_SERVICO CATEGORIA_ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_ACOMPANHAMENTO> ORDEM_SERVICO_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_ANEXO> ORDEM_SERVICO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_COMENTARIOS> ORDEM_SERVICO_COMENTARIOS { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_PRODUTO> ORDEM_SERVICO_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_SERVICO> ORDEM_SERVICO_SERVICO { get; set; }
    }
}