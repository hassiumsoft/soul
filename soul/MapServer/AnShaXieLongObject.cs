﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBase.Config;
//亡灵巫师- 暗沙邪龙
//2015.11.1
namespace MapServer
{
    public class AnShaXieLongObject : MonsterObject
    {
        private static int DIS = 20; //与宿主的距离-
        private static int REFRESHTIME = 5000; //刷新对象时间
        public PlayerObject mPlay;
        private int mnRefreshTick;
        
        public AnShaXieLongObject(PlayerObject _play, short x, short y,byte dir, uint _id, int nAi_Id)
            : base(_id, nAi_Id, x,y,false)
        {
            type = OBJECTTYPE.CALLOBJECT;
            typeid = IDManager.CreateTypeId(OBJECTTYPE.GUARDKNIGHT); 
            SetPoint(x, y);
            mRebirthTime = 0;//不允许复活
       
            mPlay = _play;
            SetDir(dir);
            mnRefreshTick = System.Environment.TickCount;
        }

        public override bool Run()
        {
            bool ret =  base.Run();
            //距离超出-
            if (!this.GetPoint().CheckVisualDistance(mPlay.GetCurrentX(), mPlay.GetCurrentY(), DIS))
            {
                mPlay.GetTimerSystem().DeleteStatus(GameStruct.RoleStatus.STATUS_ANSHAXIELONG);
                return false;
            }
            //刷新周围对象以便寻找目标
            if (this.GetAi().GetTargetObject() == null)
            {
                if (System.Environment.TickCount - mnRefreshTick > REFRESHTIME)
                {
                    this.RefreshVisibleObject();
                    mnRefreshTick = System.Environment.TickCount;
                }
            }
            return ret;
        }

        protected override void ProcessAction_Die(GameStruct.Action act)
        {
            base.ProcessAction_Die(act);
            mPlay.GetTimerSystem().DeleteStatus(GameStruct.RoleStatus.STATUS_ANSHAXIELONG);
        }

        public override void ClearThis()
        {
            this.attr.life = 0;
            base.ClearThis();
            this.GetGameMap().RemoveObj(this);
            IDManager.RecoveryTypeID(this.GetTypeId(), this.type);
        }

        public override bool CanPK(BaseObject obj, bool bGoCrime = true)
        {
            bool ret = base.CanPK(obj);
            if (ret)
            {
                //不攻击主人
                if (obj.type == OBJECTTYPE.PLAYER)
                {
                    if (obj.GetTypeId() == mPlay.GetTypeId())
                    {
                        return false;
                    }
                }
            }
            return ret;
           // return base.CheckIsAttack(obj);
        }
    }
}
