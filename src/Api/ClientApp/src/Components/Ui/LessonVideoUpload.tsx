import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { Box } from "@mantine/core";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import { FileAccess, uploadVideo } from "@utils/services/fileService";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType
);

const LessonVideoUpload = ({
  setUrl,
  currentVideo,
  marginy = 10,
}: {
  setUrl: Function;
  currentVideo?: string;
  marginy?: number;
}) => {
  useEffect(() => {
    if (currentVideo) {
      setFiles([
        {
          source: currentVideo,
          options: {
            type: "local",
          },
        },
      ]);
    }
  }, [currentVideo]);

  const [files, setFiles] = useState<any>([]);
  return (
    <Box my={marginy} sx={{ maxWidth: 470 }}>
      <FilePond
        files={files}
        onaddfile={(error, file) => {}}
        fileValidateTypeLabelExpectedTypes="Expected .mp4 .avi .mov"
        chunkSize={2 * 1024 * 1024} // 2MB
        acceptedFileTypes={["video/mp4", "video/avi", "video/mov"]}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: null,
          revert: null,
          //processing a file
          process: async (
            fieldName,
            file,
            metadata,
            load,
            error,
            progress,
            abort
          ) => {
            try {
              const res = await uploadVideo(
                file as File,
                FileAccess.Private
              );
              load(res.data);
              setUrl(() => res.data.key);
            } catch (e) {
              error("Unable to upload file");
            }
            return {
              abort: () => {
                abort();
              },
            };
          },
          load: async (source, load, error, progress, abort, headers) => {
            await fetch(
              `${source}?cache=${Math.random().toString(36).substring(2, 7)}`
            )
              .then(async (r) => {
                load(await r.blob());
              })
              .catch((r) => error(r));
            // Should expose an abort method so the request can be cancelled
            return {
              abort: () => {
                abort();
              },
            };
          },
        }}
        name="files"
        labelIdle='Drag & Drop your Video or <span class="filepond--label-action">Browse</span>'
      />
    </Box>
  );
};

export default LessonVideoUpload;
