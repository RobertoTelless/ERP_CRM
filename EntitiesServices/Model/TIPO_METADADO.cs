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
    
    public partial class TIPO_METADADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPO_METADADO()
        {
            this.METADADO = new HashSet<METADADO>();
        }
    
        public int TIME_CD_ID { get; set; }
        public string TIME_NM_NOME { get; set; }
        public int TIME_IN_DECIMAL { get; set; }
        public string TIME_MS_MASCARA { get; set; }
        public int TIME_IN_ATIVO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<METADADO> METADADO { get; set; }
    }
}
