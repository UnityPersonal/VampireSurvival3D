using Unity.Behavior;
using UnityEngine;

public interface IMonsterState 
{
    public enum StateType
    {
        Move,
        Attack,
        Die
    }
    
    public StateType MosteStateType { get; }
        
    Monster Owner { get; set; }
    void Enter();
    void Exit();
    void Execute();
    
}

public class MonsterMoveState : IMonsterState
{
    public IMonsterState.StateType MosteStateType => IMonsterState.StateType.Move;
    public Monster Owner { get; set; }
    public MonsterMoveState (Monster monster)
    {
        Owner = monster;
    }

    public void Enter()
    {
        Owner.AnimMesh.Play("Move");
        Owner.Agent.isStopped = false;
    }

    public void Exit()
    {
        Owner.Agent.isStopped = true;
    }

    public void Execute()
    {
        var myPosition = Owner.transform.position;
        var playerPoistion = Player.Instance.transform.position;
        var toPlayer = playerPoistion - myPosition;

        if (Owner.AttackRadius > toPlayer.magnitude)
        {
            Owner.Swap(IMonsterState.StateType.Attack);
        }
        else
        {
            Owner.UpdateDestination();
        }
    }
}

public class MonsterAttackState : IMonsterState
{
    public IMonsterState.StateType MosteStateType =>  IMonsterState.StateType.Attack;
    public Monster Owner { get; set; }
    public MonsterAttackState(Monster monster)
    {
        Owner = monster;
    }

    public void Enter()
    {
        Owner.IsAttacking = true;
        Owner.Agent.isStopped = false;
        Owner.AnimMesh.Play("Attack");
    }

    public void Exit()
    {
        Owner.IsAttacking = true;
        Owner.Agent.isStopped = true;
    }

    public void Execute()
    {
        
    }
}

public class MonsterDieState : IMonsterState
{
    public IMonsterState.StateType MosteStateType => IMonsterState.StateType.Die;
    public Monster Owner { get; set; }
    public MonsterDieState(Monster monster)
    {
        Owner = monster;
    }

    public void Enter()
    {
        Owner.IsDead = true;
        Owner.MonsterCollider.enabled = false;
        Owner.Agent.enabled = false;
        Owner.AnimMesh.Play("Die");
        
        GameEventManager.Publish(new DropItemEventArgs(1,Owner.transform.position));
    }

    public void Exit()
    {
    }

    public void Execute()
    {
    }
}



