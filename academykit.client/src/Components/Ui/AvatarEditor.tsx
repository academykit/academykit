import { UseFormReturnType } from "@mantine/form";
import { FileAccess, uploadFile } from "@utils/services/fileService";
import {
  default as FilePondPluginFileValidateSize,
  default as FilePondPluginImageResize,
} from "filepond-plugin-file-validate-size";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import FilePondPluginImageCrop from "filepond-plugin-image-crop";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import "filepond-plugin-image-preview/dist/filepond-plugin-image-preview.css";
import FilePondPluginImageTransform from "filepond-plugin-image-transform";
import "filepond/dist/filepond.min.css";
import { useEffect, useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import { useTranslation } from "react-i18next";

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginFileValidateSize,
  FilePondPluginImageResize,
  FilePondPluginImageCrop,
  FilePondPluginImageTransform
);

type IProps = {
  disabled: boolean;
  label?: string;
  url?: string | null;
  formContext: () => UseFormReturnType<any, (values: any) => any>;
};

const AvatarEditor = ({
  label = "files",
  url,
  formContext,
  disabled,
}: IProps) => {
  useEffect(() => {
    if (url) {
      setFiles([
        {
          source: url,
          options: {
            type: "local",
          },
        },
      ]);
    }
  }, [url]);

  const form = formContext();
  const { t } = useTranslation();
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
    imageValidateSizeLabelImageSizeTooSmall: t("img_too_small"),
  };

  return (
    <div style={{ maxWidth: 200 }}>
      <FilePond
        disabled={disabled}
        imagePreviewHeight={170}
        imageCropAspectRatio="1:1"
        styleLoadIndicatorPosition="center bottom"
        instantUpload={true}
        onremovefile={() => form.setFieldValue("imageUrl", "")}
        acceptedFileTypes={["image/png", "image/jpeg", "image/gif"]}
        // imageValidateSizeMinWidth={639}
        // imageValidateSizeMinHeight={359}
        // imageValidateSizeMinResolution={229401}
        stylePanelLayout="compact circle"
        stylePanelAspectRatio="1:1"
        files={files}
        labelIdle={`${t("drag_and_drop")} ${label} ${t(
          "or"
        )} <span class="filepond--label-action">${t("browse")}</span>`}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: null,
          revert: null,
          //processing a file
          process: async (
            _fieldName,
            file,
            _metadata,
            load,
            error,
            _progress,
            abort
          ) => {
            try {
              const res = await uploadFile(file as File, FileAccess.Public);
              load(res.data);
              form.setFieldValue("imageUrl", res.data);
            } catch (e) {
              error(t("unable_to_upload"));
            }
            return {
              abort: () => {
                abort();
              },
            };
          },
          load: async (source, load, error, _progress, abort) => {
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
        {...filePondProps}
      />
    </div>
  );
};

export default AvatarEditor;
