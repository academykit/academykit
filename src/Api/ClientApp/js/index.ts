// @ts-nocheck
const BASE_URL = ``;
const BASE_URL_Local = `http://localhost:5173`;
const api = (courseId: string, lessonId: string) =>
  `/api/course/${courseId}/lesson/${lessonId}/join`;

ZoomMtg.setZoomJSLib("https://jssdk.zoomus.cn/2.9.5/lib", "/av"); // china cdn option
ZoomMtg.preLoadWasm();
ZoomMtg.prepareJssdk();
export interface IStartExam {
  slug: string;
  roomName: string;
  jwtToken: string;
  zakToken: string;
  sdkKey: string;
  meetingId: string;
  passcode: string;
  user: {
    id: string;
    fullName?: string;
    imageUrl: string | null;
    email: string;
    mobileNumber: string;
    role: UserRole;
  };
}

const params = location.search;
const searchParams = new URLSearchParams(params);
const token = localStorage.getItem("token");
const course = searchParams.get("c") as string;
const lesson = searchParams.get("l") as string;

const leaveUrl = `/meet/${course}/${lesson}`;

function beginJoin(meetingConfig: IStartExam) {
  ZoomMtg.init({
    leaveUrl: leaveUrl + "?s=1",

    disableCORP: !window.crossOriginIsolated, // default true
    success: function () {
      ZoomMtg.i18n.load("en");
      ZoomMtg.i18n.reload("en");
      ZoomMtg.join({
        meetingNumber: meetingConfig.meetingId,
        userName: meetingConfig.user.fullName,
        signature: meetingConfig.jwtToken,
        sdkKey: meetingConfig.sdkKey,
        userEmail: meetingConfig.user.email,
        passWord: meetingConfig.passcode,
        customerKey: meetingConfig.user.id,
        success: function (res) {
          console.info("join meeting success");
          ZoomMtg.getCurrentUser({
            success: function (res) {
              console.info("success getCurrentUser", res.result.currentUser);
            },
          });
        },
        error: function (res) {
          window.location.replace(`${leaveUrl}/?s=4&e=${encodeURIComponent(
            "Something went wrong while starting meeting."
          )}
    `);
        },
      });
    },
    error: function (res) {
      window.location.replace(`${leaveUrl}/?s=4&e=${encodeURIComponent(
        "Something went wrong while starting meeting."
      )}
    `);
    },
  });
}
const fetchData = async () => {
  try {
    if (!token) {
      return window.location.replace(`${BASE_URL}/login`);
    } else if (!course && !lesson) {
      return window.location.replace(
        `${leaveUrl}/?e=${encodeURIComponent("Invalid session link")}`
      );
    } else {
      const res = await fetch(api(course, lesson), {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) throw await res.json();
      const data = (await res.json()) as IStartExam;
      beginJoin(data);
    }
  } catch (err) {
    window.location.replace(`${leaveUrl}/?s=4&e=${encodeURIComponent(
      err.message ?? "Something went wrong while starting meeting."
    )}
    `);
  }
};

fetchData();
