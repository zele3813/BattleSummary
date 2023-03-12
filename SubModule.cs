using SandBox.Missions.MissionLogics;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;


namespace BattleSummary
{
    public class SubModule : MBSubModuleBase
    {
        //TODO: Move SubModule class to a separate assembly.
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            mission.AddMissionBehavior(new BattleSummaryMissionView());
        }
    }

    public class BattleSummaryMissionView : MissionView
    {
        //Declare the fields needed to add a new UI layer to the MissionView (battle view).
        //This will be the UI to keep track of the kill counts during battle.        
        GauntletLayer _layer; 
        IGauntletMovie _movie;  
        BattleSummaryVM _dataSource;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            //Set up the new layer
            _dataSource = new BattleSummaryVM(Mission);
            _layer = new GauntletLayer(1);
            _movie = _layer.LoadMovie("BattleSummaryHUD", _dataSource);
            MissionScreen.AddLayer(_layer);
        }
        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            //Clean up the layer that was added.
            MissionScreen.RemoveLayer(_layer);
            _layer = null;
            _movie = null;
            _dataSource = null;
        }
        

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if (Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Q))
            {
 /*               var missionAgents = Mission.Current.GetMissionBehavior<MissionAgentSpawnLogic>();
                InformationManager.DisplayMessage(new InformationMessage("======================="));
                InformationManager.DisplayMessage(new InformationMessage(string.Format("Remaining troops: {0}", missionAgents.NumberOfRemainingTroops.ToString())));
                InformationManager.DisplayMessage(new InformationMessage(string.Format("Attacker troops: {0}", missionAgents.NumberOfActiveAttackerTroops.ToString())));
                InformationManager.DisplayMessage(new InformationMessage(string.Format("Defender troops: {0}", missionAgents.NumberOfActiveDefenderTroops.ToString())));

                //
                InformationManager.DisplayMessage(new InformationMessage(string.Format("Battle Size: {0}", missionAgents.BattleSize.ToString())));
                InformationManager.DisplayMessage(new InformationMessage(string.Format("Player controllable troops: {0}", missionAgents.GetNumberOfPlayerControllableTroops().ToString())));

                InformationManager.DisplayMessage(new InformationMessage(string.Format("GetAllTroopsForSide Defender: {0}", missionAgents.GetAllTroopsForSide(BattleSideEnum.Defender))));
                InformationManager.DisplayMessage(new InformationMessage(string.Format("GetAllTroopsForSide Defender: {0}", missionAgents.)));

                InformationManager.DisplayMessage(new InformationMessage("======================="));
*/
            }
        }
        

        //Add the latest kill to the counter whenever a new agent is removed from the battle field. Only tracks the ally kill count.
        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);

            if (affectorAgent == null || affectedAgent == null || affectorAgent.Team == null || affectedAgent.Team == null || !affectedAgent.IsHuman) return;

            if (affectorAgent.Team.IsPlayerAlly && !affectedAgent.Team.IsPlayerAlly)
                _dataSource.AddKill(affectorAgent.Character, true);
            else if (!affectorAgent.Team.IsPlayerAlly && affectedAgent.Team.IsPlayerAlly)
                _dataSource.AddKill(affectorAgent.Character, false);
        }

        public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {
            base.OnMissionModeChange(oldMissionMode, atStart);
            _dataSource?.OnMissionModeChanged(Mission);
        }
    }

    public class BattleSummaryVM : ViewModel
    {
        Mission _mission;

        public BattleSummaryVM(Mission mission)
        {
            _mission = mission;

            OnMissionModeChanged(mission);
        }

        public void OnMissionModeChanged(Mission mission)
        {
            IsVisible = mission.Mode is MissionMode.Battle;
        }

        bool _isVisible;
        [DataSourceProperty]
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                if (value != this._isVisible)
                {
                    this._isVisible = value;
                    base.OnPropertyChangedWithValue(value, "IsVisible");
                }
            }
        }

        //Counter properties for each category of troop type
        int _infantryKills;
        [DataSourceProperty]
        public int InfantryKills
        {
            get
            {
                return this._infantryKills;
            }
            set
            {
                if (value != this._infantryKills)
                {
                    this._infantryKills = value;
                    base.OnPropertyChangedWithValue(value, "InfantryKills");
                }
            }
        }

        int _archerKills;
        [DataSourceProperty]
        public int ArcherKills
        {
            get
            {
                return this._archerKills;
            }
            set
            {
                if (value != this._archerKills)
                {
                    this._archerKills = value;
                    base.OnPropertyChangedWithValue(value, "ArcherKills");
                }
            }
        }

        int _cavalryKills;
        [DataSourceProperty]
        public int CavalryKills
        {
            get
            {
                return this._cavalryKills;
            }
            set
            {
                if (value != this._cavalryKills)
                {
                    this._cavalryKills = value;
                    base.OnPropertyChangedWithValue(value, "CavalryKills");
                }
            }
        }

        int _cavalryArcherKills;
        [DataSourceProperty]
        public int CavalryArcherKills
        {
            get
            {
                return this._cavalryArcherKills;
            }
            set
            {
                if (value != this._cavalryArcherKills)
                {
                    this._cavalryArcherKills = value;
                    base.OnPropertyChangedWithValue(value, "CavalryArcherKills");
                }
            }
        }

        int _enemyInfantryKills;
        [DataSourceProperty]
        public int EnemyInfantryKills
        {
            get
            {
                return this._enemyInfantryKills;
            }
            set
            {
                if (value != this._enemyInfantryKills)
                {
                    this._enemyInfantryKills = value;
                    base.OnPropertyChangedWithValue(value, "EnemyInfantryKills");
                }
            }
        }

        int _enemyArcherKills;
        [DataSourceProperty]
        public int EnemyArcherKills
        {
            get
            {
                return this._enemyArcherKills;
            }
            set
            {
                if (value != this._enemyArcherKills)
                {
                    this._enemyArcherKills = value;
                    base.OnPropertyChangedWithValue(value, "EnemyArcherKills");
                }
            }
        }

        int _enemyCavalryKills;
        [DataSourceProperty]
        public int EnemyCavalryKills
        {
            get
            {
                return this._enemyCavalryKills;
            }
            set
            {
                if (value != this._enemyCavalryKills)
                {
                    this._enemyCavalryKills = value;
                    base.OnPropertyChangedWithValue(value, "EnemyCavalryKills");
                }
            }
        }

        int _enemyCavalryArcherKills;
        [DataSourceProperty]
        public int EnemyCavalryArcherKills
        {
            get
            {
                return this._enemyCavalryArcherKills;
            }
            set
            {
                if (value != this._enemyCavalryArcherKills)
                {
                    this._enemyCavalryArcherKills = value;
                    base.OnPropertyChangedWithValue(value, "EnemyCavalryArcherKills");
                }
            }
        }

        //Add ally kills to the correct troop category of counter
        //TODO: Create a separate class to keep track of the counters for ally and enemy.
        public void AddKill(BasicCharacterObject affectorTroop, bool isAlly)
        {
            if (affectorTroop == null) return;

            if (isAlly)
            {
                if (affectorTroop.IsMounted && affectorTroop.IsRanged)
                {
                    CavalryArcherKills += 1;
                }
                else if (affectorTroop.IsMounted)
                {
                    CavalryKills += 1;
                }
                else if (affectorTroop.IsRanged)
                {
                    ArcherKills += 1;

                }
                else if (affectorTroop.IsInfantry)
                {
                    InfantryKills += 1;
                }
            }
            else
            {
                if (affectorTroop.IsMounted && affectorTroop.IsRanged)
                {
                    EnemyCavalryArcherKills += 1;
                }
                else if (affectorTroop.IsMounted)
                {
                    EnemyCavalryKills += 1;
                }
                else if (affectorTroop.IsRanged)
                {
                    EnemyArcherKills += 1;

                }
                else if (affectorTroop.IsInfantry)
                {
                    EnemyInfantryKills += 1;
                }
            }
        }
    }
}
