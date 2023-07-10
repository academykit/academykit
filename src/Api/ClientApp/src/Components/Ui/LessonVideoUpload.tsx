import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import { Box, Text, Tooltip } from "@mantine/core";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import { FileAccess, uploadVideo } from "@utils/services/fileService";
import { UseFormReturnType } from "@mantine/form";
import FilePondPluginImageValidateSize from "filepond-plugin-image-validate-size";
import { useTranslation } from "react-i18next";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginImageValidateSize
);

const LessonVideoUpload = ({
  currentVideo,
  marginy = 10,
  formContext,
}: {
  currentVideo?: string;
  marginy?: number;
  formContext: () => UseFormReturnType<any, (values: any) => any>;
}) => {
  const { t } = useTranslation();

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
  const form = formContext();
  const [files, setFiles] = useState<any>([]);
  const filePondProps = {
    labelInvalidField: t("Field contains invalid files"),
    labelFileWaitingForSize: t("Waiting for size"),
    labelFileSizeNotAvailable: t("Size not available"),
    labelFileLoading: t("Loading"),
    labelFileLoadError: t("Error during load"),
    labelFileProcessing: t("Processing"),
    labelFileProcessingComplete: t("Processing complete"),
    labelFileProcessingAborted: t("Processing aborted"),
    labelFileProcessingError: t("Error during processing"),
    labelFileProcessingRevertError: t("Error during revert"),
    labelFileRemoveError: t("Error during removal"),
    labelTapToCancel: t("Tap to cancel"),
    labelTapToRetry: t("Tap to retry"),
    labelTapToUndo: t("Tap to undo"),
    labelButtonRemoveItem: t("Remove"),
    labelButtonAbortItemLoad: t("Abort"),
    labelButtonRetryItemLoad: t("Retry"),
    labelButtonAbortItemProcessing: t("Abort"),
    labelButtonUndoItemProcessing: t("Undo"),
    labelButtonRetryItemProcessing: t("Retry"),
    labelButtonProcessItem: t("Process"),
  };

  return (
    <Tooltip
      multiline
      label="Acceptable file types are [.mp4, .avi, .mov ]
            Acceptable file size is [2 GB]"
      width={320}
    >
      <Box my={marginy} sx={{ maxWidth: 470 }} pos="relative">
        <FilePond
          files={files}
          onaddfile={(error, file) => {}}
          onremovefile={() => form.setFieldValue("videoUrl", "")}
          fileValidateTypeLabelExpectedTypes="Expected .mp4 .avi .mov"
          chunkSize={2 * 1024 * 1024} // 2MB
          acceptedFileTypes={[
            "video/mp4",
            "video/avi",
            "video/mov",
            "video/quicktime",
          ]}
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
                const res = await uploadVideo(file as File, FileAccess.Private);
                load(res.data);
                form.setFieldValue("videoUrl", res.data);
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
          labelIdle={`${t(
            "video_drag_drop"
          )} <span class="filepond--label-action">${t("browse")}</span>`}
          {...filePondProps}
        />
        {form.errors["videoUrl"] && (
          <Text color={"red"} size={"xs"} pos="absolute" top={"100%"}>
            {form.errors["videoUrl"]}
          </Text>
        )}
      </Box>
    </Tooltip>
  );
};

export default LessonVideoUpload;
