import { Box, useMantineTheme } from '@mantine/core';
import Slider from 'rc-slider';
import React, { FC } from 'react';

interface Props {
  value: number;
  loadedValue: number;
  onSeek: (value: number) => void;
  className?: string;
  onSeekStart?: (value: number) => void;
  onSeekComplete?: (value: number) => void;
}
const SeekBar: FC<React.PropsWithChildren<Props>> = ({
  value,
  loadedValue,
  onSeek,
  className,
  onSeekStart,
  onSeekComplete,
}) => {
  const theme = useMantineTheme();
  return (
    <Box className={className}>
      <Slider
        style={{ width: '100%' }}
        min={1}
        max={100}
        value={value}
        trackStyle={{
          backgroundColor: theme.colors[theme.primaryColor][6],
        }}
        handleStyle={{
          borderColor: '#26ab95',
          backgroundColor: theme.primaryColor,
        }}
        railStyle={{
          borderColor: '',
          backgroundColor: theme.primaryColor[4],
          width: `${loadedValue ?? value}%`,
        }}
        onChange={(value: number | number[]) => {
          onSeek(value as number);
          onSeekStart && onSeekStart(value as number);
          onSeekComplete && onSeekComplete(value as number);
        }}
      />
    </Box>
  );
};

export default SeekBar;
