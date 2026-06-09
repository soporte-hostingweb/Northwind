document.addEventListener("DOMContentLoaded", () => {
    const counters = document.querySelectorAll('.counter');

    // Función para animar el conteo incremental
    const animateCounter = (counter) => {
        const target = +counter.getAttribute('data-target') || 0;
        let current = 0;
        
        // Define la duración total de la animación (aprox 50 frames a 60fps ~ 800ms)
        const totalSteps = 50;
        const increment = target / totalSteps;
        
        const update = () => {
            current += increment;
            if (current < target) {
                counter.innerText = Math.ceil(current);
                requestAnimationFrame(update);
            } else {
                counter.innerText = target;
            }
        };
        
        update();
    };

    // Opciones del observador de intersección
    const observerOptions = {
        threshold: 0.15 // Se activa cuando el 15% del elemento es visible en pantalla
    };

    // Inicializa el IntersectionObserver
    const observer = new IntersectionObserver((entries, self) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const counterElement = entry.target;
                
                // Agrega clase CSS de transición (opacidad y traslación)
                counterElement.classList.add('visible');
                
                // Ejecuta la animación de los números
                animateCounter(counterElement);
                
                // Deja de observar para que la animación no se repita en scrolls posteriores
                self.unobserve(counterElement);
            }
        });
    }, observerOptions);

    // Configura e inicia la observación de cada contador
    counters.forEach(counter => {
        counter.innerText = '0'; // Forzar inicio en 0
        observer.observe(counter);
    });
});