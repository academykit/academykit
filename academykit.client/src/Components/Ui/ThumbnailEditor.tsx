import useCustomForm from '@hooks/useCustomForm';
import { Text } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form/lib/types';
import { FileAccess, uploadFile } from '@utils/services/fileService';
import FilePondPluginFileValidateSize from 'filepond-plugin-file-validate-size';
import FilePondPluginFileValidateType from 'filepond-plugin-file-validate-type';
import FilePondPluginImageExifOrientation from 'filepond-plugin-image-exif-orientation';
import FilePondPluginImagePreview from 'filepond-plugin-image-preview';
import 'filepond-plugin-image-preview/dist/filepond-plugin-image-preview.css';
import FilePondPluginImageResize from 'filepond-plugin-image-resize';
import FilePondPluginImageTransform from 'filepond-plugin-image-transform';
import FilePondPluginImageValidateSize from 'filepond-plugin-image-validate-size';
import 'filepond/dist/filepond.min.css';
import { useEffect, useState } from 'react';
import { FilePond, registerPlugin } from 'react-filepond';
import { useTranslation } from 'react-i18next';

registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginFileValidateType,
  FilePondPluginFileValidateSize,
  FilePondPluginImageResize,
  FilePondPluginImageValidateSize,
  FilePondPluginImageTransform
);
type IProps = {
  formContext: () => UseFormReturnType<any, (values: any) => any>;
  label?: string;
  FormField?: string;
  currentThumbnail?: string;
  width?: string;
  disabled?: boolean;
};

const ThumbnailEditor = ({
  formContext,
  label = 'files',
  FormField = 'thumbnail',
  currentThumbnail,
  width,
  disabled = false,
}: IProps) => {
  const cForm = useCustomForm();

  const { t } = useTranslation();

  useEffect(() => {
    if (currentThumbnail) {
      setFiles([
        {
          source: currentThumbnail,
          options: {
            type: 'local',
          },
        },
      ]);
    }
  }, [currentThumbnail]);
  const form = formContext();
  const [files, setFiles] = useState<any>([]);
  const filePondProps = {
    labelInvalidField: t('Field contains invalid files'),
    labelFileWaitingForSize: t('Waiting for size'),
    labelFileSizeNotAvailable: t('Size not available'),
    labelFileLoading: t('Loading'),
    labelFileLoadError: t('Error during load'),
    labelFileProcessing: t('Processing'),
    labelFileProcessingComplete: t('Processing complete'),
    labelFileProcessingAborted: t('Processing aborted'),
    labelFileProcessingError: t('Error during processing'),
    labelFileProcessingRevertError: t('Error during revert'),
    labelFileRemoveError: t('Error during removal'),
    labelTapToCancel: t('Tap to cancel'),
    labelTapToRetry: t('Tap to retry'),
    labelTapToUndo: t('Tap to undo'),
    labelButtonRemoveItem: t('Remove'),
    labelButtonAbortItemLoad: t('Abort'),
    labelButtonRetryItemLoad: t('Retry'),
    labelButtonAbortItemProcessing: t('Abort'),
    labelButtonUndoItemProcessing: t('Undo'),
    labelButtonRetryItemProcessing: t('Retry'),
    labelButtonProcessItem: t('Process'),
    imageValidateSizeLabelImageSizeTooSmall: t('img_too_small'),
  };

  return (
    <div
      style={{
        maxWidth: width ? '' : 470,
        position: 'relative',
        width: width ? width : '',
      }}
    >
      <FilePond
        disabled={disabled}
        instantUpload={true}
        acceptedFileTypes={[
          'image/png',
          'image/jpeg',
          'image/gif',
          'image/jpg',
        ]}
        files={files}
        labelIdle={`${t(
          'drag_and_drop'
        )} ${label} or <span class="filepond--label-action">${t(
          'browse'
        )}</span>`}
        imageValidateSizeMinWidth={639}
        imageValidateSizeMinHeight={359}
        allowImageResize={true}
        imageValidateSizeMinResolution={229401}
        onaddfile={() => {}}
        onupdatefiles={setFiles}
        // onremovefile={() => form.setFieldValue(FormField, '')}
        onremovefile={() => {}}
        allowMultiple={false}
        imageResizeTargetWidth={1280}
        imageResizeTargetHeight={700}
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
            cForm?.setReady();
            try {
              const res = await uploadFile(file as File, FileAccess.Public);
              load(res.data);
              form.setFieldValue(FormField, res.data);
            } catch (e) {
              error(t('unable_to_upload'));
            }
            cForm?.setReady();

            return {
              abort: () => {
                abort();
              },
            };
          },
          load: async (source, load, error, _progress, abort) => {
            cForm?.setReady();
            await fetch(
              `${source}?cache=${Math.random().toString(36).substring(2, 7)}`
            )
              .then(async (r) => {
                load(await r.blob());
                cForm?.setReady();
              })
              .catch((r) => {
                error(r);
                cForm?.setReady();
              });
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
      {form.errors[FormField] && (
        <Text c={'red'} size={'xs'} pos="absolute" top={'100%'}>
          {form.errors[FormField]}
        </Text>
      )}
    </div>
  );
};

export default ThumbnailEditor;
