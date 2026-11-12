import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import { applyTheme, themes, defaultThemeName } from './theme';
import './styles/global.css';

applyTheme(themes[defaultThemeName]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
