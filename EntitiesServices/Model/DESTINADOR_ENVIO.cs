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
    
    public partial class DESTINADOR_ENVIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DESTINADOR_ENVIO()
        {
            this.DESTINADOR_ENVIO_PRODUTO = new HashSet<DESTINADOR_ENVIO_PRODUTO>();
            this.DESTINADOR_ENVIO_ANOTACOES = new HashSet<DESTINADOR_ENVIO_ANOTACOES>();
            this.DESTINADOR_ENVIO_ANEXO = new HashSet<DESTINADOR_ENVIO_ANEXO>();
        }
    
        public int DEEN_CD_ID { get; set; }
        public int DEST_CD_ID { get; set; }
        public int PRES_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public string DEEN_NM_DESCRICAO { get; set; }
        public System.DateTime DEEN_DT_CADASTRO { get; set; }
        public byte[] DEEN_TX_OBSERVACAO { get; set; }
        public int DEEN_IN_ATIVO { get; set; }
    
        public virtual DESTINADOR DESTINADOR { get; set; }
        public virtual PRESTADOR PRESTADOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESTINADOR_ENVIO_PRODUTO> DESTINADOR_ENVIO_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESTINADOR_ENVIO_ANOTACOES> DESTINADOR_ENVIO_ANOTACOES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESTINADOR_ENVIO_ANEXO> DESTINADOR_ENVIO_ANEXO { get; set; }
    }
}
