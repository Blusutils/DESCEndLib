using System.Reflection;
using DESCEnd.Logging;

namespace DESCEnd {
    /// <summary>
    /// Represents the method that executes on <see cref="CEnd.Run(CEndTargetDelegate)"/>
    /// </summary>
    public delegate void CEndTargetDelegate();
    /// <summary>
    /// Attribute for methods that can be runned in <see cref="CEnd"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event)]
    public class CEndTargetAttribute : Attribute {
        bool restartOnError = false;
        /// <summary>
        /// a
        /// </summary>
        /// <param name="restartOnError">Need to restart thread when exception raises?</param>
        public CEndTargetAttribute(bool restartOnError = true) {
            this.restartOnError = restartOnError;
        }
    }
    /// <summary>
    /// what is dis sh*t
    /// </summary>
    public class CEndThread {
        /// <summary>
        /// Name of thread
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is thread alive and running?
        /// </summary>
        public bool IsAlive { get { return innerThread.IsAlive; } }
        /// <summary>
        /// what is dis???
        /// </summary>
        Thread innerThread;
        /// <summary>
        /// why
        /// </summary>
        object arg;
        /// <summary>
        /// Create new instance of <see cref="CEndThread"/>
        /// </summary>
        /// <param name="toRun">Method to run</param>
        /// <param name="arg">idk what is this</param>
        public CEndThread(ParameterizedThreadStart toRun, object arg = null) {
            innerThread = new Thread(toRun);
            this.arg = arg;
        }
        /// <summary>
        /// Create new instance of <see cref="CEndThread"/>
        /// </summary>
        /// <param name="toRun">Method to run</param>
        /// <param name="arg">idk what is this</param>
        public CEndThread(ThreadStart toRun, object arg = null) {
            innerThread = new Thread(toRun);
            this.arg = arg;
        }
        /// <summary>
        /// Start the thread
        /// </summary>
        /// <param name="arg">Arguments (???)</param>
        public void Start(object arg = null) {
            this.arg = arg ?? this.arg;
            innerThread.Start(this.arg);
        }
        /// <summary>
        /// Abort thread run
        /// </summary>
        public void Abort() {
            innerThread.Abort();
        }
        /// <summary>
        /// Join to thread and block calling thread until this thread won't ended
        /// </summary>
        public void Join() {
            innerThread.Join();
        }

    }

    /// <summary>
    /// DESrv CEnd (Controlled End) class
    /// </summary>
    public class CEnd {
        /// <summary>
        /// Logger
        /// </summary>
        public static CEndLog Logger;
        /// <summary>
        /// For <see cref="Target(object)"/>
        /// </summary>
        Exception? runResult = null;
        Assembly targetAsm;
        /// <summary>
        /// Create a new instance of <see cref="CEnd"/>
        /// </summary>
        public CEnd() {
            Logger = Logger ?? new CEndLog();
            targetAsm = Assembly.GetExecutingAssembly();
        }
        /// <summary>
        /// Create a new instance of <see cref="CEnd"/> with logger
        /// </summary>
        /// <param name="log"><see cref="CEndLog"/> logger</param>
        public CEnd(Assembly asm, CEndLog log=null) {
            Logger = log??Logger??new CEndLog();
            targetAsm = asm??Assembly.GetExecutingAssembly();
        }
        /// <summary>
        /// Target method runner
        /// </summary>
        /// <param name="target">Target method as <see cref="CEndTargetDelegate"/></param>
        void Target(object target) {
            // i added this method only for handling exceptions in thread
            try {
                if (target is CEndTargetDelegate trg)
                    trg();
            } catch (Exception ex) {
                runResult = ex;
                //logger.Critical($"Error in {ex.Source}: {ex.Message}\n{ex.StackTrace}");
            }
        }
        /// <summary>
        /// Prepares <see cref="CEndTargetDelegate"/> to run in thread
        /// </summary>
        /// <param name="target">Target method</param>
        /// <param name="name">Thread name</param>
        /// <returns>Prepared thread</returns>
        CEndThread PrepareThread(CEndTargetDelegate target, string name) {
            var thr = new CEndThread(Target);
            thr.Name = name;
            thr.Start(target);
            return thr;
        }
        /// <summary>
        /// Run method in <see cref="CEnd"/>
        /// </summary>
        /// <param name="target">Target method</param>
        /// <param name="restartOnError">Need to restart thread when exception raises?</param>
        public void Run(CEndTargetDelegate target, bool restartOnError = true) {
            var name = "DESCEnd-" + Guid.NewGuid();
            var thr = PrepareThread(target, name);
            Run(thr, target, restartOnError);
        }
        /// <summary>
        /// Run thread in <see cref="CEnd"/>
        /// </summary>
        /// <param name="targetThread">Target thread</param>
        private void Run(CEndThread targetThread, CEndTargetDelegate targetMethod, bool restartOnError = true, int fails = 0) {
            if (!targetThread.Name.StartsWith("DESCEnd-"))
                targetThread.Name = "DESCEnd-" + Guid.NewGuid() + "-CN-" + targetThread.Name.Replace(" ", "-");
            try {
                if (!targetThread.IsAlive) targetThread.Start();
            } catch (ThreadStateException) {
                targetThread = PrepareThread(targetMethod, targetThread.Name);
            }
            Logger.Info($"Thread {targetThread.Name} started");
            targetThread.Join();
            if (runResult != null) {
                Logger.Error($"Thread {targetThread.Name} failed (from method {runResult.TargetSite}, caused by {runResult.Source}). Exception: {runResult.GetType()}: {runResult.Message}\nStack trace: \t{runResult.StackTrace}");
                if (fails < 6 && restartOnError) {
                    Logger.Warn($"Restarting thread {targetThread.Name}");
                    Thread.Sleep(1000);
                    Run(targetThread, targetMethod, fails: fails + 1);
                } else { Logger.Critical($"Maximum restart attempts retrieved for {targetThread.Name}. Aborting."); }
            };
        }
        /// <summary>
        /// Runs all elements tagged by <see cref="CEndTargetAttribute"/>
        /// </summary>
        public void RunTagged() {
            foreach (var t in targetAsm.GetTypes()) {
                foreach (var fe in t.GetEvents()) {
                    if (fe.GetCustomAttribute<CEndTargetAttribute>() != null) {
                        ThreadStart cb = () => {
                            fe.RaiseMethod.Invoke(fe.RaiseMethod.IsStatic ? null : fe, null);
                        };
                        Run(new CEndThread(cb), null);
                    }
                }
                foreach (var fm in t.GetMethods()) {
                    if (fm.GetCustomAttribute<CEndTargetAttribute>() != null) {
                        ThreadStart cb = () => {
                            fm.Invoke(fm.IsStatic ? null : t, null);
                        };
                        Run(new CEndThread(cb), null);
                    }
                }
            }
        }
    }
}
