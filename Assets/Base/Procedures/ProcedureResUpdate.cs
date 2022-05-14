using Global;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using UGFHelper;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System.IO;

namespace Base
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcedureResUpdate : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }
    }
}
