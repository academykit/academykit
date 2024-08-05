import { FormContext } from '@context/FormContext';
import { useContext } from 'react';

const useCustomForm = () => {
  return useContext(FormContext);
};

export default useCustomForm;
