import Slider from 'rc-slider';
import React, { FC } from 'react';

interface Props {
  currentVolume: number;
  className?: string;
  onValueChanged?: (value: number) => void;
}

const VolumeControlBar: FC<React.PropsWithChildren<Props>> = ({
  currentVolume,
  className,
  onValueChanged,
}) => {
  return (
    <div className={className}>
      <Slider
        min={0}
        max={100}
        value={currentVolume}
        trackStyle={{
          backgroundColor: '#26ab95',
        }}
        handleStyle={{
          borderColor: '#26ab95',
          backgroundColor: '#26ab95',
        }}
        onChange={(value: number | number[]) =>
          onValueChanged && onValueChanged(value as number)
        }
      />
    </div>
  );
};

export default VolumeControlBar;
