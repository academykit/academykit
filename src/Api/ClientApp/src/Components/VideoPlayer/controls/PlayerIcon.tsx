/* eslint-disable */
import { Text } from '@mantine/core';

const Play = ({ ...props }) => {
  return (
    <>
      <Text hidden>Play</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        style={{ padding: '2px' }}
        {...props}
      >
        <path d="M5 3L19 12 5 21 5 3z"></path>
      </svg>
    </>
  );
};

const Pause = ({ ...props }) => {
  return (
    <>
      <Text hidden>Pause</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        className="mx-auto"
        style={{ padding: '2px' }}
        {...props}
      >
        <path d="M6 4H10V20H6z"></path>
        <path d="M14 4H18V20H14z"></path>
      </svg>
    </>
  );
};

const Settings = ({ ...props }) => {
  return (
    <>
      <Text hidden>Settings</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        style={{ padding: '5px' }}
        className="mx-auto"
        {...props}
      >
        <circle cx="12" cy="12" r="3"></circle>
        <path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-2 2 2 2 0 01-2-2v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83 0 2 2 0 010-2.83l.06-.06a1.65 1.65 0 00.33-1.82 1.65 1.65 0 00-1.51-1H3a2 2 0 01-2-2 2 2 0 012-2h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 010-2.83 2 2 0 012.83 0l.06.06a1.65 1.65 0 001.82.33H9a1.65 1.65 0 001-1.51V3a2 2 0 012-2 2 2 0 012 2v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 0 2 2 0 010 2.83l-.06.06a1.65 1.65 0 00-.33 1.82V9a1.65 1.65 0 001.51 1H21a2 2 0 012 2 2 2 0 01-2 2h-.09a1.65 1.65 0 00-1.51 1z"></path>
      </svg>
    </>
  );
};

const PiP = ({ ...props }) => {
  return (
    <>
      <Text hidden>Exit mini player</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="currentColor"
        viewBox="0 0 16 16"
        className="mx-auto"
        {...props}
      >
        <path d="M0 3.5A1.5 1.5 0 011.5 2h13A1.5 1.5 0 0116 3.5v9a1.5 1.5 0 01-1.5 1.5h-13A1.5 1.5 0 010 12.5v-9zM1.5 3a.5.5 0 00-.5.5v9a.5.5 0 00.5.5h13a.5.5 0 00.5-.5v-9a.5.5 0 00-.5-.5h-13z"></path>
        <path d="M8 8.5a.5.5 0 01.5-.5h5a.5.5 0 01.5.5v3a.5.5 0 01-.5.5h-5a.5.5 0 01-.5-.5v-3z"></path>
      </svg>
    </>
  );
};

const PiPExit = ({ ...props }) => {
  return (
    <>
      <Text hidden>Exit Mini player</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="16"
        height="16"
        fill="currentColor"
        viewBox="0 0 16 16"
        className="mx-auto"
        {...props}
      >
        <path d="M1.5 2A1.5 1.5 0 000 3.5v9A1.5 1.5 0 001.5 14h13a1.5 1.5 0 001.5-1.5v-9A1.5 1.5 0 0014.5 2h-13zm7 6h5a.5.5 0 01.5.5v3a.5.5 0 01-.5.5h-5a.5.5 0 01-.5-.5v-3a.5.5 0 01.5-.5z"></path>
      </svg>
    </>
  );
};

const FullScreen = ({ ...props }) => {
  return (
    <>
      <Text hidden>Full screen</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="30"
        height="30"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        style={{ padding: '5px' }}
        className="mx-auto"
        {...props}
      >
        <path d="M15 3L21 3 21 9"></path>
        <path d="M9 21L3 21 3 15"></path>
        <path d="M21 3L14 10"></path>
        <path d="M3 21L10 14"></path>
      </svg>
    </>
  );
};

const FullScreenExit = ({ ...props }) => {
  return (
    <>
      <Text hidden>Exit full screen</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="27"
        height="27"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        style={{ padding: '3px' }}
        className="mx-auto"
        {...props}
      >
        <path d="M4 14L10 14 10 20"></path>
        <path d="M20 10L14 10 14 4"></path>
        <path d="M14 10L21 3"></path>
        <path d="M3 21L10 14"></path>
      </svg>
    </>
  );
};

const Rewind = ({ ...props }) => {
  return (
    <>
      <Text hidden>Rewind</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        {...props}
      >
        <path d="M11 19L2 12 11 5 11 19z"></path>
        <path d="M22 19L13 12 22 5 22 19z"></path>
      </svg>
    </>
  );
};

const FastForward = ({ ...props }) => {
  return (
    <>
      <Text hidden>Fast forward</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        {...props}
      >
        <path d="M13 19L22 12 13 5 13 19z"></path>
        <path d="M2 19L11 12 2 5 2 19z"></path>
      </svg>
    </>
  );
};

const Volume = ({ ...props }) => {
  return (
    <>
      <Text hidden>Mute</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        style={{ padding: '2px' }}
        {...props}
      >
        <path d="M11 5L6 9 2 9 2 15 6 15 11 19 11 5z"></path>
        <path d="M19.07 4.93a10 10 0 010 14.14M15.54 8.46a5 5 0 010 7.07"></path>
      </svg>
    </>
  );
};

const VolumeMuted = ({ ...props }) => {
  return (
    <>
      <Text hidden>unmute</Text>

      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        viewBox="0 0 24 24"
        aria-hidden="true"
        style={{ padding: '2px' }}
        {...props}
      >
        <path d="M11 5L6 9 2 9 2 15 6 15 11 19 11 5z"></path>
        <path d="M23 9L17 15"></path>
        <path d="M17 9L23 15"></path>
      </svg>
    </>
  );
};

const BigCirclePlay = ({ ...props }) => {
  return (
    <>
      <Text hidden>Play</Text>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="100"
        height="100"
        fill="none"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        className="text-primary"
        viewBox="0 0 24 24"
        {...props}
      >
        <circle cx="12" cy="12" r="10" stroke="white"></circle>
        <path d="M10 8L16 12 10 16 10 8z" stroke="currentColor"></path>
      </svg>
    </>
  );
};

const PlayerIcon = {
  Play,
  Pause,
  Settings,
  Volume,
  VolumeMuted,
  FullScreen,
  FullScreenExit,
  PiP,
  PiPExit,
  Rewind,
  FastForward,
  BigCirclePlay,
};

export default PlayerIcon;
