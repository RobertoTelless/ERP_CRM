using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_CRM_Solution.ViewModels
{
    public class CargoViewModel
    {
        [Key]
        public int CARG_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string CARG_NM_NOME { get; set; }
        public Nullable<int> CARG_IN_ATIVO { get; set; }
        public Nullable<int> CARG_IN_TIPO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }

    }
}