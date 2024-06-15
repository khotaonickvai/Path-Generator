

using System.Collections.Generic;

public abstract class Command
{
   private bool _isRedoCommand;
   private static Stack<Command> _executedCommands;
   public static Stack<Command> ExecutedCommands
   {
      get
      {
         if (_executedCommands == null)
            _executedCommands = new Stack<Command>();
         return _executedCommands;
      }
   }

   private static Stack<Command> _undoCommand;

   public static Stack<Command> UndoCommands
   {
      get
      {
         if (_undoCommand == null)
            _undoCommand = new Stack<Command>();
         return _undoCommand;
      }
      
   }
   public virtual void Execute()
   {
      ExecutedCommands.Push(this);
      if(this._isRedoCommand == false)
         UndoCommands.Clear();
   }

   public static void Undo()
   {
      if (ExecutedCommands.Count > 0)
      {
         ExecutedCommands.Pop().Revert();
      }
   }

   public static void Redo()
   {
      if (UndoCommands.Count > 0)
      {
         UndoCommands.Pop().Execute();
      }
   }
   
   
   public virtual void Revert()
   {
      this._isRedoCommand = true;
      UndoCommands.Push(this);
   }
}
