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

        /*protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);

            var campaignStarter = starterObject as CampaignGameStarter;
            if (campaignStarter != null)
            {
                campaignStarter.AddBehavior(new )
            }

        }*/
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

        //Add the latest kill to the counter whenever a new agent is removed from the battle field. Only tracks the ally kill count as of now.
        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);

            //InformationManager.DisplayMessage(new InformationMessage("In MissionView OnAgentRemoved Override."));
            
            if (affectorAgent != null && affectedAgent != null)
            {
                if (affectorAgent.Team.IsPlayerAlly) _dataSource.AddKill(affectorAgent.Character, affectedAgent.Character);
            }
            //TODO: Add kill counters for enemy
            //TODO: Display the remaining reinforcement troop count
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

        //Add to the right category of counter
        //TODO: Create a separate class to keep track of the counters.
        public void AddKill(BasicCharacterObject affectorTroop, BasicCharacterObject affectedTroop)
        {
            if (affectorTroop == null) return;

            if (affectorTroop.IsMounted && affectorTroop.IsRanged)
            {
                CavalryArcherKills += 1;
                //InformationManager.DisplayMessage(new InformationMessage("Cavalry archer kill detected."));
            }
            else if (affectorTroop.IsMounted)
            {
                CavalryKills += 1;
                //InformationManager.DisplayMessage(new InformationMessage("Cavalry kill detected."));
            }
            else if (affectorTroop.IsRanged)
            {
                ArcherKills += 1;
                //InformationManager.DisplayMessage(new InformationMessage("Archer kill detected."));
            }else
            {
                InfantryKills += 1;
                //InformationManager.DisplayMessage(new InformationMessage("Infantry kill detected."));
            }
        }
    }
}
