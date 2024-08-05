import { Loader } from '@mantine/core';
import Copy from './Copy';

const LoadingAndCopy = ({
  loading,
  value,
}: {
  loading: boolean;
  value: string;
}) => {
  return loading ? <Loader size={16} /> : <Copy value={value} />;
};

export default LoadingAndCopy;
