import { useEffect } from 'react';

const Hangfire = () => {
  useEffect(() => {
    // navigate('https://standalone.apps.vurilo.com/hangfire');
    window.location.replace('https://standalone.apps.vurilo.com/hangfire');
  }, []);
  return <></>;
};

export default Hangfire;
