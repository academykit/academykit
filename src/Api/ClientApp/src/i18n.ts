import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import en from './config/language/en.json';
import np from './config/language/np.json';
import jp from './config/language/jp.json';

i18n.use(initReactI18next).init({
  lng: 'en',
  fallbackLng: 'en',
  interpolation: {
    escapeValue: false,
  },
  resources: {
    en: { translation: en },
    ne: { translation: np },
    ja: { translation: jp },
  },
});

export default i18n;
