//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GRUPO_DOCUMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GRUPO_DOCUMENTO()
        {
            this.CLASSE = new HashSet<CLASSE>();
            this.DOCUMENTO = new HashSet<DOCUMENTO>();
        }
    
        public int GRDC_CD_ID { get; set; }
        public string GRDC_NM_NOME { get; set; }
        public int GRDC_IN_ATIVO { get; set; }
        public int ASSI_CD_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLASSE> CLASSE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTO> DOCUMENTO { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}
