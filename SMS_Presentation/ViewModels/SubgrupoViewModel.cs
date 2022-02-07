using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class SubgrupoViewModel
    {
        [Key]
        public int SUBG_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> GRUP_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo GRUPO obrigatorio")]
        public Nullable<int> GRCC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 no máximo 50 caracteres.")]
        public string SUBG_NM_NOME { get; set; }
        public Nullable<int> SUBG_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string SUBG_NR_NUMERO { get; set; }
        public string SUBG_NM_EXIBE { get; set; }
        [StringLength(50, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 50 caracteres.")]
        public string SUBG_DS_DESCRICAO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CENTRO_CUSTO> CENTRO_CUSTO { get; set; }
        public virtual GRUPO GRUPO { get; set; }
        public virtual GRUPO_CC GRUPO_CC { get; set; }
    }
}