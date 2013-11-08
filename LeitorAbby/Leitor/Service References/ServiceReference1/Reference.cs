﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Leitor.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IConsumoArquivos")]
    public interface IConsumoArquivos {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConsumoArquivos/GetStatusWCF", ReplyAction="http://tempuri.org/IConsumoArquivos/GetStatusWCFResponse")]
        void GetStatusWCF();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IConsumoArquivos/GetStatusWCF", ReplyAction="http://tempuri.org/IConsumoArquivos/GetStatusWCFResponse")]
        System.IAsyncResult BeginGetStatusWCF(System.AsyncCallback callback, object asyncState);
        
        void EndGetStatusWCF(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConsumoArquivos/UploadZipFile", ReplyAction="http://tempuri.org/IConsumoArquivos/UploadZipFileResponse")]
        string UploadZipFile(byte[] fileByteArray, string fileName);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IConsumoArquivos/UploadZipFile", ReplyAction="http://tempuri.org/IConsumoArquivos/UploadZipFileResponse")]
        System.IAsyncResult BeginUploadZipFile(byte[] fileByteArray, string fileName, System.AsyncCallback callback, object asyncState);
        
        string EndUploadZipFile(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IConsumoArquivosChannel : Leitor.ServiceReference1.IConsumoArquivos, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UploadZipFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public UploadZipFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConsumoArquivosClient : System.ServiceModel.ClientBase<Leitor.ServiceReference1.IConsumoArquivos>, Leitor.ServiceReference1.IConsumoArquivos {
        
        private BeginOperationDelegate onBeginGetStatusWCFDelegate;
        
        private EndOperationDelegate onEndGetStatusWCFDelegate;
        
        private System.Threading.SendOrPostCallback onGetStatusWCFCompletedDelegate;
        
        private BeginOperationDelegate onBeginUploadZipFileDelegate;
        
        private EndOperationDelegate onEndUploadZipFileDelegate;
        
        private System.Threading.SendOrPostCallback onUploadZipFileCompletedDelegate;
        
        public ConsumoArquivosClient() {
        }
        
        public ConsumoArquivosClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConsumoArquivosClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConsumoArquivosClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConsumoArquivosClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> GetStatusWCFCompleted;
        
        public event System.EventHandler<UploadZipFileCompletedEventArgs> UploadZipFileCompleted;
        
        public void GetStatusWCF() {
            base.Channel.GetStatusWCF();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetStatusWCF(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetStatusWCF(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndGetStatusWCF(System.IAsyncResult result) {
            base.Channel.EndGetStatusWCF(result);
        }
        
        private System.IAsyncResult OnBeginGetStatusWCF(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return this.BeginGetStatusWCF(callback, asyncState);
        }
        
        private object[] OnEndGetStatusWCF(System.IAsyncResult result) {
            this.EndGetStatusWCF(result);
            return null;
        }
        
        private void OnGetStatusWCFCompleted(object state) {
            if ((this.GetStatusWCFCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetStatusWCFCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetStatusWCFAsync() {
            this.GetStatusWCFAsync(null);
        }
        
        public void GetStatusWCFAsync(object userState) {
            if ((this.onBeginGetStatusWCFDelegate == null)) {
                this.onBeginGetStatusWCFDelegate = new BeginOperationDelegate(this.OnBeginGetStatusWCF);
            }
            if ((this.onEndGetStatusWCFDelegate == null)) {
                this.onEndGetStatusWCFDelegate = new EndOperationDelegate(this.OnEndGetStatusWCF);
            }
            if ((this.onGetStatusWCFCompletedDelegate == null)) {
                this.onGetStatusWCFCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetStatusWCFCompleted);
            }
            base.InvokeAsync(this.onBeginGetStatusWCFDelegate, null, this.onEndGetStatusWCFDelegate, this.onGetStatusWCFCompletedDelegate, userState);
        }
        
        public string UploadZipFile(byte[] fileByteArray, string fileName) {
            return base.Channel.UploadZipFile(fileByteArray, fileName);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginUploadZipFile(byte[] fileByteArray, string fileName, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginUploadZipFile(fileByteArray, fileName, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public string EndUploadZipFile(System.IAsyncResult result) {
            return base.Channel.EndUploadZipFile(result);
        }
        
        private System.IAsyncResult OnBeginUploadZipFile(object[] inValues, System.AsyncCallback callback, object asyncState) {
            byte[] fileByteArray = ((byte[])(inValues[0]));
            string fileName = ((string)(inValues[1]));
            return this.BeginUploadZipFile(fileByteArray, fileName, callback, asyncState);
        }
        
        private object[] OnEndUploadZipFile(System.IAsyncResult result) {
            string retVal = this.EndUploadZipFile(result);
            return new object[] {
                    retVal};
        }
        
        private void OnUploadZipFileCompleted(object state) {
            if ((this.UploadZipFileCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.UploadZipFileCompleted(this, new UploadZipFileCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void UploadZipFileAsync(byte[] fileByteArray, string fileName) {
            this.UploadZipFileAsync(fileByteArray, fileName, null);
        }
        
        public void UploadZipFileAsync(byte[] fileByteArray, string fileName, object userState) {
            if ((this.onBeginUploadZipFileDelegate == null)) {
                this.onBeginUploadZipFileDelegate = new BeginOperationDelegate(this.OnBeginUploadZipFile);
            }
            if ((this.onEndUploadZipFileDelegate == null)) {
                this.onEndUploadZipFileDelegate = new EndOperationDelegate(this.OnEndUploadZipFile);
            }
            if ((this.onUploadZipFileCompletedDelegate == null)) {
                this.onUploadZipFileCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnUploadZipFileCompleted);
            }
            base.InvokeAsync(this.onBeginUploadZipFileDelegate, new object[] {
                        fileByteArray,
                        fileName}, this.onEndUploadZipFileDelegate, this.onUploadZipFileCompletedDelegate, userState);
        }
    }
}
