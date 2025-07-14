// Audio Companion - Spectrum Analyzer JavaScript

window.initializeSpectrumCanvas = (canvas) => {
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    
    // Store context reference
    canvas._audioContext = ctx;
    
    // Set up canvas properties
    const rect = canvas.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;
    
    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;
    
    ctx.scale(dpr, dpr);
    canvas.style.width = rect.width + 'px';
    canvas.style.height = rect.height + 'px';
    
    // Initial clear
    clearSpectrum(canvas);
};

window.updateSpectrum = (canvas, spectrumData) => {
    if (!canvas || !canvas._audioContext || !spectrumData) return;
    
    const ctx = canvas._audioContext;
    const width = canvas.width / (window.devicePixelRatio || 1);
    const height = canvas.height / (window.devicePixelRatio || 1);
    
    // Clear canvas
    ctx.fillStyle = '#1a1a1a';
    ctx.fillRect(0, 0, width, height);
    
    // Draw frequency grid
    drawFrequencyGrid(ctx, width, height);
    
    // Draw spectrum
    drawSpectrum(ctx, spectrumData, width, height);
    
    // Draw frequency labels
    drawFrequencyLabels(ctx, width, height);
};

window.clearSpectrum = (canvas) => {
    if (!canvas || !canvas._audioContext) return;
    
    const ctx = canvas._audioContext;
    const width = canvas.width / (window.devicePixelRatio || 1);
    const height = canvas.height / (window.devicePixelRatio || 1);
    
    // Clear with background color
    ctx.fillStyle = '#1a1a1a';
    ctx.fillRect(0, 0, width, height);
    
    // Draw grid
    drawFrequencyGrid(ctx, width, height);
    drawFrequencyLabels(ctx, width, height);
};

function drawFrequencyGrid(ctx, width, height) {
    ctx.strokeStyle = '#444444';
    ctx.lineWidth = 1;
    
    // Horizontal lines (dB levels)
    const dbLevels = [-60, -40, -20, -6, 0];
    dbLevels.forEach(db => {
        const y = height - ((db + 60) / 60) * height;
        ctx.beginPath();
        ctx.moveTo(0, y);
        ctx.lineTo(width, y);
        ctx.stroke();
    });
    
    // Vertical lines (frequency bands)
    const freqPositions = [0.1, 0.25, 0.4, 0.6, 0.8]; // Approximate frequency positions
    freqPositions.forEach(pos => {
        const x = pos * width;
        ctx.beginPath();
        ctx.moveTo(x, 0);
        ctx.lineTo(x, height);
        ctx.stroke();
    });
}

function drawSpectrum(ctx, spectrumData, width, height) {
    if (!spectrumData || spectrumData.length === 0) return;
    
    const barWidth = width / spectrumData.length;
    
    for (let i = 0; i < spectrumData.length; i++) {
        const value = spectrumData[i];
        const db = value > 0 ? Math.max(-60, 20 * Math.log10(value)) : -60;
        const barHeight = ((db + 60) / 60) * height;
        
        // Color based on level
        let color;
        if (db > -6) {
            color = '#f44336'; // Red - danger zone
        } else if (db > -20) {
            color = '#ffeb3b'; // Yellow - caution
        } else {
            color = '#4caf50'; // Green - safe
        }
        
        ctx.fillStyle = color;
        ctx.fillRect(i * barWidth, height - barHeight, barWidth - 1, barHeight);
    }
}

function drawFrequencyLabels(ctx, width, height) {
    ctx.fillStyle = '#cccccc';
    ctx.font = '12px Arial';
    ctx.textAlign = 'center';
    
    // Frequency labels
    const labels = [
        { text: '20Hz', pos: 0.1 },
        { text: '200Hz', pos: 0.25 },
        { text: '2kHz', pos: 0.4 },
        { text: '8kHz', pos: 0.6 },
        { text: '20kHz', pos: 0.8 }
    ];
    
    labels.forEach(label => {
        const x = label.pos * width;
        ctx.fillText(label.text, x, height - 5);
    });
    
    // dB labels
    ctx.textAlign = 'right';
    const dbLabels = [
        { text: '0dB', db: 0 },
        { text: '-6dB', db: -6 },
        { text: '-20dB', db: -20 },
        { text: '-40dB', db: -40 },
        { text: '-60dB', db: -60 }
    ];
    
    dbLabels.forEach(label => {
        const y = height - ((label.db + 60) / 60) * height;
        ctx.fillText(label.text, width - 5, y - 5);
    });
}
