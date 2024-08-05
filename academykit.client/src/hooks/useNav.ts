import { NavContext } from '@context/NavContext';
import { useContext } from 'react';

const useNav = () => useContext(NavContext);
export default useNav;
