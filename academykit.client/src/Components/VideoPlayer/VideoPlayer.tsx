import { ActionIcon, Box, Flex } from "@mantine/core";
import cx from "clsx";
import fscreen from "fscreen";
import "rc-slider/assets/index.css";
import React, { FC, useEffect, useRef, useState } from "react";
import ReactPlayer, { Config, ReactPlayerProps } from "react-player";
import PlayerIcon from "./controls/PlayerIcon";
import RemainingTimeDisplay from "./controls/RemaningTimeDisplay";
import SeekBar from "./controls/SeekBar";
import VolumeControlBar from "./controls/VolumeControlBar";
import classes from "./styles/player.module.css";
interface Props extends ReactPlayerProps {
  url?: string;
  config?: Config;
  thumbnailUrl?: string;
  setCurrentPlayerState: React.Dispatch<
    React.SetStateAction<
      | "loading"
      | "completed"
      | "loaded"
      | "playing"
      | "paused"
      | "viewing"
      | "buffering"
    >
  >;
}

const VideoPlayer: FC<React.PropsWithChildren<Props>> = ({
  url,
  config,
  thumbnailUrl,
  onEnded,
  setCurrentPlayerState,
}) => {
  const playerRef = useRef<ReactPlayer>(null);
  const [playing, setPlaying] = useState(false);
  const [controls] = useState(false);
  const [playsinline] = useState(false);
  const [pip, setPip] = useState(false);
  const [playbackRate] = useState(1);
  const [loop] = useState(false);
  const [volume, setVolume] = useState(0.8);
  const [prevVolume, setPrevVolume] = useState(volume);
  const [muted, setMuted] = useState(false);
  const [, setSeeking] = useState(false);
  const [played, setPlayed] = useState(0);
  const [loadedValue, setLoadedValue] = useState(0);
  const [hideControls, setHideControls] = useState(false);
  const [duration, setDuration] = useState(0);
  const [inFullscreenMode, setInFullscreenMode] = React.useState(false);

  config = config ?? {
    file: {
      attributes: {
        crossOrigin: "anonymous",
        controlsList: "nodownload",
        onContextMenu: (e: any) => e.preventDefault(),
      },
    },
  };

  const togglePlay = () => {
    setPlaying(!playing);
  };
  useEffect(() => {
    setPip(pip);
  }, [pip]);

  const seek = (value: number) => {
    setPlayed(value / 100);
  };

  const handleSeekStart = () => {
    setSeeking(true);
  };

  const handleSeekComplete = (newValue: number) => {
    setSeeking(false);
    playerRef.current?.seekTo(newValue / 100, "fraction");
  };

  const handleVolumeChange = (value: number) => {
    if (value == 0) {
      setMuted(true);
    } else {
      setMuted(false);
    }
    setVolume(() => value / 100.0);
    setPrevVolume(volume);
  };

  const toggleMute = () => {
    setPrevVolume(() => volume);
    if (muted) {
      setVolume(prevVolume);
    } else {
      setVolume(0);
    }
    setMuted(!muted);
  };

  const handleProgress = (state: {
    played: number;
    playedSeconds: number;
    loaded: number;
    loadedSeconds: number;
  }) => {
    // if (!seeking) {
    setPlayed(state.played);
    setLoadedValue(state.loaded);
    // if (onProgress && isFunction(onProgress)) {
    //   onProgress(state);
    // }
    // setCurrentPlayerState("playing");
    // }
  };

  const handleReady = () => {
    setCurrentPlayerState("loading");
  };

  const onVideoCompleted = () => {
    setPlaying(false);
    setCurrentPlayerState("completed");
    if (onEnded) {
      onEnded();
    }
  };

  const handleTogglePIP = () => {
    setInFullscreenMode(false);
    setPip(!pip);
  };

  const handleDisablePiP = () => {
    setPip(false);
  };

  const [hideControlTimerFunc, setHideControlTimerFunc] =
    useState<NodeJS.Timeout | null>(null);

  const autoHideControls = (hide: boolean) => {
    if (hideControlTimerFunc) clearTimeout(hideControlTimerFunc);
    if (playing === true && hide) {
      setHideControlTimerFunc(
        setTimeout(() => {
          setHideControls(true);
        }, 1000)
      );
    } else {
      setHideControls(false);
    }
  };

  const wrapperElement = useRef<HTMLDivElement>(null!);

  const toggleFullscreen = () => {
    if (inFullscreenMode) {
      fscreen.exitFullscreen();
      setInFullscreenMode(false);
    } else {
      fscreen.requestFullscreen(wrapperElement.current);
      setPip(false);
      setInFullscreenMode(true);
    }
  };
  return (
    <Box
      ref={wrapperElement}
      onMouseMove={() => {
        autoHideControls(false);
      }}
      onMouseLeave={() => {
        autoHideControls(true);
      }}
      style={{
        heightL: "100%",
        width: "100%",
        position: "relative",
      }}
    >
      <Flex
        direction={"column"}
        style={{ width: "100%", height: "100%" }}
        onClick={() => {
          togglePlay();
        }}
      >
        <ReactPlayer
          ref={playerRef}
          width="100%"
          height="100%"
          url={url}
          pip={pip}
          playing={playing}
          controls={controls}
          playsinline={playsinline}
          light={thumbnailUrl}
          loop={loop}
          playbackRate={playbackRate}
          volume={volume}
          muted={muted}
          progressInterval={1000}
          stopOnUnmount={!pip}
          onReady={handleReady}
          onStart={() => setCurrentPlayerState("viewing")}
          onPlay={() => {
            setPlaying(true);
            setCurrentPlayerState("playing");
          }}
          onEnablePIP={() => setPip(true)}
          onDisablePIP={handleDisablePiP}
          onPause={() => {
            setPlaying(false);
            setCurrentPlayerState("paused");
          }}
          onBuffer={() => setCurrentPlayerState("buffering")}
          onEnded={onVideoCompleted}
          onClickPreview={() => setPlaying(true)}
          onProgress={handleProgress}
          onDuration={(duration: number) => {
            setDuration(duration);
            setCurrentPlayerState("loaded");
          }}
          config={config}
          playIcon={<PlayerIcon.BigCirclePlay />}
        ></ReactPlayer>
      </Flex>
      {url && loadedValue > 0 && (
        <Box
          className={cx(classes.seekWrapper, {
            [classes.hidden]: hideControls,
          })}
        >
          <ActionIcon
            variant="subtle"
            color=""
            onClick={() => setPlaying(!playing)}
          >
            {!playing ? <PlayerIcon.Play /> : <PlayerIcon.Pause />}
          </ActionIcon>
          <SeekBar
            className={classes.seek}
            value={played * 100}
            onSeek={seek}
            loadedValue={loadedValue * 100}
            onSeekStart={handleSeekStart}
            onSeekComplete={handleSeekComplete}
          />
          <RemainingTimeDisplay duration={duration} played={played} />
          <Flex
            style={{
              justifyContent: "center",
              alignItems: "center",
              paddingRight: 8,
            }}
          >
            <ActionIcon
              variant="subtle"
              color={"white"}
              mx={5}
              onClick={toggleMute}
            >
              {muted ? <PlayerIcon.VolumeMuted /> : <PlayerIcon.Volume />}
            </ActionIcon>
            <VolumeControlBar
              className={classes.volume}
              currentVolume={volume * 100}
              onValueChanged={handleVolumeChange}
            ></VolumeControlBar>
          </Flex>
          {ReactPlayer.canEnablePIP(url) && (
            <ActionIcon
              variant="subtle"
              mx={3}
              color={"white"}
              onClick={handleTogglePIP}
            >
              {pip ? <PlayerIcon.PiPExit /> : <PlayerIcon.PiP />}
            </ActionIcon>
          )}
          {fscreen.fullscreenEnabled && (
            <ActionIcon
              variant="subtle"
              mx={3}
              color={"white"}
              onClick={toggleFullscreen}
            >
              {inFullscreenMode ? (
                <PlayerIcon.FullScreenExit />
              ) : (
                <PlayerIcon.FullScreen />
              )}
            </ActionIcon>
          )}
        </Box>
      )}
    </Box>
  );
};

export default VideoPlayer;
