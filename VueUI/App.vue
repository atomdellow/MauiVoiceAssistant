<template>
  <div class="voice-assistant">
    <div class="container">
      <h1 class="title">Voice Assistant</h1>
      
      <div class="mic-section">
        <button 
          class="mic-button"
          :class="{ 'recording': isRecording, 'thinking': isThinking }"
          @mousedown="startRecording"
          @mouseup="stopRecording"
          @touchstart="startRecording"
          @touchend="stopRecording"
          :disabled="isThinking"
        >
          <div class="mic-icon">
            <svg v-if="!isRecording && !isThinking" width="60" height="60" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 2c1.1 0 2 .9 2 2v6c0 1.1-.9 2-2 2s-2-.9-2-2V4c0-1.1.9-2 2-2zm6 6c0 3.31-2.69 6-6 6s-6-2.69-6-6H4c0 3.52 2.61 6.43 6 6.92V21h4v-2.08c3.39-.49 6-3.4 6-6.92h-2z"/>
            </svg>
            <div v-else-if="isRecording" class="recording-animation">
              <div class="pulse"></div>
              <div class="pulse"></div>
              <div class="pulse"></div>
            </div>
            <div v-else class="thinking-spinner">
              <div class="spinner"></div>
            </div>
          </div>
        </button>
        
        <p class="instruction">
          {{ isRecording ? 'Recording... Release to stop' : 
             isThinking ? 'Thinking...' : 
             'Hold to speak' }}
        </p>
      </div>
      
      <div class="reply-section" v-if="lastReply">
        <div class="reply-bubble">
          <p>{{ lastReply }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'App',
  data() {
    return {
      isRecording: false,
      isThinking: false,
      lastReply: ''
    }
  },
  mounted() {
    // Listen for replies from MAUI
    window.onAssistantReply = (text) => {
      this.lastReply = text
      this.isThinking = false
    }
  },
  methods: {
    startRecording() {
      if (this.isThinking) return
      
      this.isRecording = true
      this.lastReply = ''
      
      // Send action to MAUI
      if (window.hybridBridge) {
        window.hybridBridge.sendAction('start-recording')
      }
    },
    
    stopRecording() {
      if (!this.isRecording) return
      
      this.isRecording = false
      this.isThinking = true
      
      // Send action to MAUI
      if (window.hybridBridge) {
        window.hybridBridge.sendAction('stop-recording')
      }
    }
  }
}
</script>

<style scoped>
.voice-assistant {
  width: 100%;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-align: center;
  padding: 20px;
}

.container {
  max-width: 400px;
  width: 100%;
}

.title {
  font-size: 2.5rem;
  font-weight: 300;
  margin-bottom: 3rem;
  text-shadow: 0 2px 4px rgba(0,0,0,0.3);
}

.mic-section {
  margin-bottom: 3rem;
}

.mic-button {
  width: 160px;
  height: 160px;
  border-radius: 50%;
  border: none;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  box-shadow: 0 8px 32px rgba(0,0,0,0.3);
}

.mic-button:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
  box-shadow: 0 12px 40px rgba(0,0,0,0.4);
}

.mic-button.recording {
  background: rgba(220, 53, 69, 0.8);
  animation: pulse 1.5s infinite;
}

.mic-button.thinking {
  background: rgba(255, 193, 7, 0.8);
  cursor: not-allowed;
}

.mic-button:disabled {
  opacity: 0.7;
}

.mic-icon {
  display: flex;
  align-items: center;
  justify-content: center;
}

.recording-animation {
  display: flex;
  gap: 8px;
}

.pulse {
  width: 12px;
  height: 12px;
  background: white;
  border-radius: 50%;
  animation: pulse-dot 1.4s infinite ease-in-out both;
}

.pulse:nth-child(2) {
  animation-delay: -0.16s;
}

.pulse:nth-child(3) {
  animation-delay: -0.32s;
}

.thinking-spinner {
  display: flex;
  align-items: center;
  justify-content: center;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.instruction {
  font-size: 1.1rem;
  opacity: 0.9;
  font-weight: 300;
}

.reply-section {
  margin-top: 2rem;
}

.reply-bubble {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 1.5rem;
  box-shadow: 0 4px 20px rgba(0,0,0,0.2);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.reply-bubble p {
  font-size: 1.1rem;
  line-height: 1.6;
  margin: 0;
}

@keyframes pulse {
  0% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
  100% {
    transform: scale(1);
  }
}

@keyframes pulse-dot {
  0%, 80%, 100% {
    transform: scale(0);
  }
  40% {
    transform: scale(1);
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Mobile responsiveness */
@media (max-width: 480px) {
  .title {
    font-size: 2rem;
    margin-bottom: 2rem;
  }
  
  .mic-button {
    width: 140px;
    height: 140px;
  }
  
  .container {
    padding: 0 10px;
  }
}
</style>